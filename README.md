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
		
To launch the driving car controled with python script:

	To activate python env cariables:
		activate ADAS_Tester

	To launch the python script:
		ADAS_Tester\Assets\SampleScenes\Python
	Type:
		python drive.py
		
		
	
