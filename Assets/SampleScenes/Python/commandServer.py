import base64
import os
import socketio
import eventlet
import eventlet.wsgi
from PIL import Image
from flask import Flask
from io import BytesIO

import drive

sio = socketio.Server()
app = Flask(__name__)
model = None
prev_image_array = None


@sio.on('connect')
def connect(sid, environ):
    sendDriveInfo(sio, 0, 0, 0)

@sio.on('toExtMsg')
def toExtMsg(sid, data):
    if data:
        msgHeader = data["messageHeader"]
        msgSize = data["messageSize"]
        
        if (msgHeader == "telemetry"):
            receivedTelemetry(data)
        
        if (msgHeader == "cameraImg"):
            receivedCameraImg(data)
            
        if (msgHeader == "lidarInfo"):
            receivedLidarInfo(data)
    else:
        sendEmptyInfo(sio)
        
def receivedTelemetry(data):        
    steering_angle = float(data["0"])
    throttle = float(data["1"])
    speed = float(data["2"])
    distanceToWalker = float(data["3"])
    
    drive.getAdasOrder(sio, steering_angle, throttle, speed, distanceToWalker)


def receivedCameraImg(data):
    image = Image.open(BytesIO(base64.b64decode(data["0"])))

def receivedLidarInfo(data):
    msgSize = int(data["messageSize"])
    receivedCoord = []
    for coord in range(0, msgSize):
        receivedCoord.append(float(data[str(coord)]))
    for coord in range(0, int(msgSize/4)):
        print (str(receivedCoord[4*coord]) + " " + str(receivedCoord[4*coord+1]) + " " + str(receivedCoord[4*coord+2]) + " " + str(receivedCoord[4*coord+3]))
        
        

def sendDriveInfo(sio, steering_angle, throttle, pedestrian):
    msgHeader = "driveInfo"
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
    msgHeader = "emptyInfo"
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

    
    