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
import commandServer

if (ADAS_ALGO_SRC == "_python_"):
    from adasAlgo import detect_pedestrian
    
if (ADAS_ALGO_SRC == "_octave_"):   
    from oct2py import octave
    cwd = os.getcwd()
    octave.addpath(cwd) 
    
if (ADAS_ALGO_SRC == "_matlab_"):
    import matlab.engine
    

MAX_SPEED = 25
MIN_SPEED = 10
speed_limit = MAX_SPEED


def getAdasOrder(sio, steering_angle, throttle, speed, distanceToWalker):
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
        commandServer.sendDriveInfo(sio, steering_angle, throttle, pedestrian)
    except Exception as e:
        print(e)
                        
