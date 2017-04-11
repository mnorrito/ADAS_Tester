from oct2py import octave
#import matlab.engine


#import argparse
import base64
#from datetime import datetime
import os
#import shutil

#import numpy as np
import socketio
import eventlet
import eventlet.wsgi
from PIL import Image
from flask import Flask
from io import BytesIO

#from keras.models import load_model
#import utils
cwd = os.getcwd()
octave.addpath(cwd)

sio = socketio.Server()
app = Flask(__name__)
model = None
prev_image_array = None

MAX_SPEED = 25
MIN_SPEED = 10

speed_limit = MAX_SPEED

@sio.on('toExtMsg')
def toExtMsg(sid, data):
    if data:
        msgHeader = data["messageHeader"]
        msgSize = data["messageSize"]
        
        if (msgHeader == "telemetry"):
            receivedTelemetry(data)
        
        if (msgHeader == "cameraImg"):
            receivedCameraImg(data)
            
    else:
        sendEmptyInfo()
        
def receivedTelemetry(data):        
    if data:
        steering_angle = float(data["0"])
        throttle = float(data["1"])
        speed = float(data["2"])
        distanceToWalker = float(data["3"])
        pedestrian = 0
        
        try:
            global speed_limit
            if speed > speed_limit:
                speed_limit = MIN_SPEED  # slow down
            else:
                speed_limit = MAX_SPEED
            #throttle = 1.0 - steering_angle**2 - (speed/speed_limit)**2
            throttle = 1.0 - (speed/speed_limit)**2
            throttle = octave.detect_pedestrian(distanceToWalker, throttle, speed)

            print('{} {} {} {}'.format(steering_angle, throttle, speed, pedestrian))
            sendDriveInfo(steering_angle, throttle, pedestrian)
        except Exception as e:
            print(e)

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
                        
def receivedCameraImg(data):
    msgHeader = "driveInfo"
    msgSize = 3
	#image = Image.open(BytesIO(base64.b64decode(data["image"])))

@sio.on('connect')
def connect(sid, environ):
    print("connect ", sid)
    sendDriveInfo(0, 0, 0)

if __name__ == '__main__':
    # wrap Flask application with engineio's middleware
    app = socketio.Middleware(sio, app)
    # deploy as an eventlet WSGI server
    eventlet.wsgi.server(eventlet.listen(('', 4567)), app)
