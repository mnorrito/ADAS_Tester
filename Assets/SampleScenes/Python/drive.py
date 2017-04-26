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
    print("ADAS Algo scr = PYTHON")
    
if (ADAS_ALGO_SRC == "_octave_"):   
    from oct2py import octave
    cwd = os.getcwd()
    octave.addpath(cwd) 
    print("ADAS Algo scr = OCTAVE")
    
if (ADAS_ALGO_SRC == "_matlab_"):
    import matlab.engine
    eng = matlab.engine.start_matlab()
    print("ADAS Algo scr = MATLAB")

MAX_SPEED = 25
MIN_SPEED = 10
speed_limit = MAX_SPEED


def dstToWlakerAlgo(steering_angle, throttle, speed, distanceToWalker):
    try:
        if (ADAS_ALGO_SRC == "_python_"):
            throttle, pedestrian = detect_pedestrian(distanceToWalker, throttle, speed)
        if (ADAS_ALGO_SRC == "_octave_"):
            throttle = octave.detect_pedestrian(distanceToWalker, throttle, speed)
        if (ADAS_ALGO_SRC == "_matlab_"):
            eng.detect_pedestrian(distanceToWalker, throttle, speed)
        pedestrian = 0
        #print('{} {} {} {}'.format(steering_angle, throttle, speed, pedestrian))
        #commandServer.sendDriveInfo(sio, steering_angle, throttle, pedestrian)
    except Exception as e:
        print(e)
    return steering_angle, throttle, pedestrian
    
def lidarAlgo(steering_angle, throttle, speed, receivedCoord):
    pedestrian = 0
    print("------------------------------------")
    for elem in receivedCoord:
        print(str(elem))
    print("------------------------------------")       
    return steering_angle, throttle, pedestrian
    
def speedRegul(throttle, speed):
    global speed_limit
    if speed > speed_limit:
        speed_limit = MIN_SPEED  # slow down
    else:
        speed_limit = MAX_SPEED
    throttle = 1 - (speed/speed_limit)**2
    if throttle < 0:
        throttle=0
    return throttle