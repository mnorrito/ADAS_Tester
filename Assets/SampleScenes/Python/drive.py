#ADAS_ALGO_SRC = "_octave_"
#ADAS_ALGO_SRC = "_python_"
ADAS_ALGO_SRC = "_matlab_"

import base64
import os
import socketio
import eventlet
import eventlet.wsgi
from PIL import Image
from flask import Flask
from io import BytesIO
import commandServer
import numpy as np

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
    matSessions = matlab.engine.find_matlab()
    if (len(matSessions) == 0):
        print("Starting new Matlab session")
        eng = matlab.engine.start_matlab()
    else:
        print("Sharing existing Matlab session: " + str(matSessions[0]))
        future=matlab.engine.connect_matlab(async=True)
        eng=future.result()
    
    lidMatrixInit = np.zeros((1,120))
    eng.workspace['lidMatrix'] = lidMatrixInit.ravel().tolist()
    eng.sim("lidarUseModel",async=True)
    eng.set_param('lidarUseModel','Solver','ode15s','StopTime','0.1',nargout=0)
    eng.set_param('lidarUseModel','Solver','ode15s','StopTime','inf',nargout=0)
    eng.set_param('lidarUseModel','SimulationCommand','start',async=True, nargout=0)

    # imageInit = np.zeros((1,921600))
    # eng.workspace['image'] = imageInit.ravel().tolist()
    # eng.sim("cameraUseModel",async=True)
    # eng.set_param('cameraUseModel','Solver','ode15s','StopTime','0.1',nargout=0)
    # eng.set_param('cameraUseModel','Solver','ode15s','StopTime','inf',nargout=0)
    # eng.set_param('cameraUseModel','SimulationCommand','start',async=True, nargout=0)

    
    print("ADAS Algo scr = MATLAB")

MAX_SPEED = 12
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
    
def imageAlgo(image_array):
    pedestrian = 0
    if (ADAS_ALGO_SRC == "_matlab_"):
        shape = image_array.shape
        image = image_array.ravel().tolist()
        ### USE MATLAB .M FILE
        #pedestrian = eng.imageUse(shape[0], shape[1], image)
        
        ### USE SIMULINK MODEL
        eng.set_param('cameraUseModel','SimulationCommand','pause',async=True, nargout=0)
        eng.workspace['image'] = image
        eng.set_param('cameraUseModel','SimulationCommand','continue',async=True,nargout=0)
        pedestrianVec  = eng.eval('pedestrian.Data(size(pedestrian.Data))')
        pedestrian = pedestrianVec[0][0]
        print("pedestrian =" + str(pedestrian))

    return pedestrian
    
    
def lidarAlgo(receivedCoord):
    pedestrian = 0
    #print("------------------------------------")
    #for elem in receivedCoord:
        #print(str(elem))
    #print("------------------------------------")       
    if (ADAS_ALGO_SRC == "_octave_"):
        pedestrian = octave.lidarUse_oct(receivedCoord)
    if (ADAS_ALGO_SRC == "_matlab_"):
        ### USE MATLAB .M FILE
        #pedestrian = eng.lidarUse(receivedCoord)
        
        ### USE SIMULINK MODEL
        eng.set_param('lidarUseModel','SimulationCommand','pause',async=True, nargout=0)
        eng.workspace['lidMatrix'] = receivedCoord
        eng.set_param('lidarUseModel','SimulationCommand','continue',async=True,nargout=0)
        pedestrianVec  = eng.eval('pedestrian.Data(size(pedestrian.Data))')
        pedestrian = pedestrianVec[0][0]
        print("pedestrian =" + str(pedestrian))
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