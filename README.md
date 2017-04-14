"# ADAS_Tester" 
Adas tester based on Unity tool
Based on 
- Unity Standard assets
- Udacity self driving car project:
    https://github.com/udacity/self-driving-car-sim 
- Convolutional network for end-to-end driving in a simulator using Tensorflow and Keras (Naoki Shibuya)
    https://github.com/naokishibuya/car-behavioral-cloning

    
INSTALLATION GUIDE

- Download and install Unity
    https://store.unity.com/

- Download Python (anaconda)
    https://www.continuum.io/downloads

- In a conda prompt window
    Go to:
        ADAS_Tester\Assets\SampleScenes\Python
    Type:
        conda env create -f environments.yml
    To allow octave functions call from python scripts:
        conda install -c conda-forge oct2py
    
    To allow Matlab functions call from python scripts:     
        cd [matlabroot]\extern\engines\python
        python setup.py install

- Add Octave path in the PATH environement variable
    set PATH=[PathToOCtave];%PATH%
        

USER'S GUIDE
- To launch the driving car controled with python script:
    - Activate python env variables (only once per session): activate ADAS_Tester
    - Set ADAS_ALGO_SRC in 'ADAS_Tester\Assets\SampleScenes\Python\drive.py' by uncommenting the desired line
    - Launch python script => python commandServer.py
        
- To launch the driving car in autonomous mode, select 'Standalone' in Unity UI (Parameters->Parameters_Car)


    
