import base64
import os
import socketio
import eventlet
import eventlet.wsgi
from PIL import Image
from flask import Flask
from io import BytesIO
from enum import Enum
import drive

sio = socketio.Server()
app = Flask(__name__)
model = None
prev_image_array = None

#Global variables
steering_angle = 0
throttle = 0
speed = 0
pedestrian = 0

class MsgHeaderType(Enum):
    telemetry = 0
    lidarInfo = 1
    cameraImg = 2
    driveInfo = 3
    emptyInfo = 4

@sio.on('connect')
def connect(sid, environ):
    sendDriveInfo(sio, 0, 0, 0)

@sio.on('toExtMsg')
def toExtMsg(sid, data):
    if data:
        msgHeader = int(data["messageHeader"])
        if (msgHeader == int(MsgHeaderType.telemetry.value)):
            receivedTelemetry(data)
        if (msgHeader == int(MsgHeaderType.lidarInfo.value)):
            receivedLidarInfo(data)        
        if (msgHeader == int(MsgHeaderType.cameraImg.value)):
            receivedCameraImg(data)
        
def receivedTelemetry(data):        
    print(">>> Received telemetry")
    global steering_angle
    global throttle
    global speed
    global pedestrian
    #steering_angle = float(data["0"])
    #throttle = float(data["1"])
    speed = float(data["2"])
    distanceToWalker = float(data["3"])
    sendDriveInfo(sio, steering_angle, throttle, pedestrian)    
    throttle = drive.speedRegul(throttle, speed)
    steering_angle, throttle, pedestrian = drive.dstToWlakerAlgo(steering_angle, throttle, speed, distanceToWalker)
    

def receivedCameraImg(data):
    #print(">>> Received image")
    imageName = data["0"]
    image = Image.open(BytesIO(base64.b64decode(data["0"])))
    throttle = drive.speedRegul(throttle, speed)
    sendDriveInfo(sio, steering_angle, throttle, pedestrian)

def receivedLidarInfo(data):
    #print(">>> Received lidar info")
    global steering_angle
    global throttle
    global speed
    global pedestrian
    msgSize = int(data["messageSize"])
    receivedCoord = []
    for coord in range(0, msgSize):
        receivedCoord.append(float(data[str(coord)]))
    #for coord in range(0, int(msgSize/4)):
        #print (str(receivedCoord[4*coord]) + " " + str(receivedCoord[4*coord+1]) + " " + str(receivedCoord[4*coord+2]) + " " + str(receivedCoord[4*coord+3]))
    throttle = drive.speedRegul(throttle, speed)        
    steering_angle, throttle, pedestrian = drive.lidarAlgo(steering_angle, throttle, speed, receivedCoord)
    sendDriveInfo(sio, steering_angle, throttle, pedestrian)
    
def sendDriveInfo(sio, steering_angle, throttle, pedestrian):
    #print("<<< Sending drive info")
    msgHeader = MsgHeaderType.driveInfo.value
    msgSize = 3
    sio.emit(
        "toUnityMsg",
        data={
            'messageHeader': msgHeader.__str__(),
            'messageSize': msgSize.__str__(),
            '0': steering_angle.__str__(),
            '1': throttle.__str__(),
            '2': pedestrian.__str__()
        },
        skip_sid=True)
            
def sendEmptyInfo(sio):
    #print("<<< Sending empty info")
    msgHeader = MsgHeaderType.emptyInfo.value
    msgSize = 0
    sio.emit(
        "toUnityMsg",
        data={
            'messageHeader': msgHeader.__str__(),
            'messageSize': msgSize.__str__(),
        },
        skip_sid=True)
 
    
        

 
if __name__ == '__main__':
    # wrap Flask application with engineio's middleware
    app = socketio.Middleware(sio, app)
    # deploy as an eventlet WSGI server
    eventlet.wsgi.server(eventlet.listen(('', 4567)), app)

    
    