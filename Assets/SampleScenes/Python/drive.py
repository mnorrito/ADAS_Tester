ADAS_ALGO_SRC = "_octave_"
#ADAS_ALGO_SRC = "_python_"
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

MAX_SPEED = 15
MIN_SPEED = 10
speed_limit = MAX_SPEED


def dstToWlakerAlgo(distanceToWalker):
    pedestrian = 0
    try:
        if (ADAS_ALGO_SRC == "_python_"):
            pedestrian = detect_pedestrian(distanceToWalker)
        if (ADAS_ALGO_SRC == "_octave_"):
            pedestrian = octave.detect_pedestrian(distanceToWalker)
        if (ADAS_ALGO_SRC == "_matlab_"):
            pedestrian = eng.detect_pedestrian(distanceToWalker)
        #print('{} {} {} {}'.format(steering_angle, throttle, speed, pedestrian))
    except Exception as e:
        print(e)
    return pedestrian
    
def lidarAlgo(receivedCoord):
    pedestrian = 0
    #print("------------------------------------")
    #for elem in receivedCoord:
        #print(str(elem))
    #print("------------------------------------")       
    if (ADAS_ALGO_SRC == "_octave_"):
        pedestrian = octave.lidarUse(receivedCoord)

    return pedestrian
    
def speedRegul(speed, pedestrian):
    global speed_limit
    if speed > speed_limit:
        speed_limit = MIN_SPEED  # slow down
    else:
        speed_limit = MAX_SPEED
    throttle = 1 - (speed/speed_limit)**2
    if throttle < 0:
        throttle=0
    if pedestrian == 1:
        throttle = -0.8
        if speed < 3:
            throttle = 0
    return throttle