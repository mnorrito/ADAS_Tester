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
		
To launch the driving car controled with python script:
	Activate python env cariables (only once per session):
		activate ADAS_Tester

	To launch ADAS algo in python from the python script:
		In ADAS_Tester\Assets\SampleScenes\Python
	Type:
		python drivePython.py
		
	To launch ADAS algo in octave from the python script:
		In ADAS_Tester\Assets\SampleScenes\Python
	Type:
		python driveOctave.py
		
	
