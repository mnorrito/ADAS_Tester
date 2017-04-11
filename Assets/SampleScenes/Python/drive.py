#ADAS_ALGO_SRC = "_octave_"
ADAS_ALGO_SRC = "_python_"
#ADAS_ALGO_SRC = "_matlab_"

import base64
import os
import socketio
import eventlet
import eventlet.wsgi
from PIL import Image
from flask import Flask
from io import BytesIO


if (ADAS_ALGO_SRC == "_python_"):
    from adasAlgo import detect_pedestrian
    
if (ADAS_ALGO_SRC == "_octave_"):   
    from oct2py import octave
    cwd = os.getcwd()
    octave.addpath(cwd) 
    
if (ADAS_ALGO_SRC == "_matlab_"):
    import matlab.engine
    

sio = socketio.Server()
app = Flask(__name__)
model = None
prev_image_array = None

MAX_SPEED = 25
MIN_SPEED = 10
speed_limit = MAX_SPEED

@sio.on('connect')
def connect(sid, environ):
    print("connect ", sid)
    sendDriveInfo(0, 0, 0)

@sio.on('toExtMsg')
def toExtMsg(sid, data):
    if data:
        msgHeader = data["messageHeader"]
        msgSize = data["messageSize"]
        
        if (msgHeader == "telemetry"):
            receivedTelemetry(data)
        
        if (msgHeader == "cameraImg"):
            receivedCameraImg(data)
            
        if (msgHeader == "lidaInfo"):
            receivedLidarInfo(data)
    else:
        sendEmptyInfo()
        
def receivedTelemetry(data):        
    steering_angle = float(data["0"])
    throttle = float(data["1"])
    speed = float(data["2"])
    distanceToWalker = float(data["3"])
    
    getAdasOrder(steering_angle, throttle, speed, distanceToWalker)


def receivedCameraImg(data):
    image = Image.open(BytesIO(base64.b64decode(data["0"])))

def receivedLidarInfo(data):
    msgSize = int(data["messageSize"])

    
def sendDriveInfo(steering_angle, throttle, pedestrian):
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
            
def sendEmptyInfo():
    msgHeader = "emptyInfo"
    msgSize = 0
    sio.emit(
        "toUnityMsg",
        data={
            'messageHeader': msgHeader.__str__(),
            'messageSize': msgSize.__str__(),
        },
        skip_sid=True)
                        
def getAdasOrder(steering_angle, throttle, speed, distanceToWalker):
    try:
        global speed_limit
        if speed > speed_limit:
            speed_limit = MIN_SPEED  # slow down
        else:
            speed_limit = MAX_SPEED
        #throttle = 1.0 - steering_angle**2 - (speed/speed_limit)**2
        throttle = 1.0 - (speed/speed_limit)**2
        if (ADAS_ALGO_SRC == "_python_"):
            throttle, pedestrian = detect_pedestrian(distanceToWalker, throttle, speed)
            
        if (ADAS_ALGO_SRC == "_octave_"):
            throttle = octave.detect_pedestrian(distanceToWalker, throttle, speed)
        pedestrian = 0
        print('{} {} {} {}'.format(steering_angle, throttle, speed, pedestrian))
        sendDriveInfo(steering_angle, throttle, pedestrian)
    except Exception as e:
        print(e)
                        
if __name__ == '__main__':
    # wrap Flask application with engineio's middleware
    app = socketio.Middleware(sio, app)
    # deploy as an eventlet WSGI server
    eventlet.wsgi.server(eventlet.listen(('', 4567)), app)

    
    