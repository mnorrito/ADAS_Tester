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



@sio.on('telemetry')
def telemetry(sid, data):
    if data:
        # The current steering angle of the car
        steering_angle = float(data["steering_angle"])
        # The current throttle of the car
        throttle = float(data["throttle"])
        # The current speed of the car
        speed = float(data["speed"])
        # The current image from the center camera of the car
        image = Image.open(BytesIO(base64.b64decode(data["image"])))
        distanceToWalker = float(data["distanceToWalker"])
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
            #throttle = octave.getThrottle(distanceToWalker, throttle, speed)
            #pedestrian = octave.getPedestrian(distanceToWalker, throttle, speed)
            
            #eng.detect_pedestrian(distanceToWalker, throttle, speed)
            
            print('{} {} {} {}'.format(steering_angle, throttle, speed, pedestrian))
            send_control(steering_angle, throttle, pedestrian)
        except Exception as e:
            print(e)


    else:
        # NOTE: DON'T EDIT THIS.
        sio.emit('manual', data={}, skip_sid=True)


@sio.on('connect')
def connect(sid, environ):
    print("connect ", sid)
    send_control(0, 0, 0)
    #eng = matlab.engine.start_matlab()

def send_control(steering_angle, throttle, pedestrian):
    print("sending control ", steering_angle, throttle, pedestrian)
    sio.emit(
        "steer",
        data={
            'steering_angle': steering_angle.__str__(),
            'throttle': throttle.__str__(),
            'pedestrian': pedestrian.__str__()
        },
        skip_sid=True)


if __name__ == '__main__':
    # wrap Flask application with engineio's middleware
    app = socketio.Middleware(sio, app)
    # deploy as an eventlet WSGI server
    eventlet.wsgi.server(eventlet.listen(('', 4567)), app)
