function [pedestrianDetected] = lidarUse(lidMatrix)
	echo off all
  len = length(lidMatrix)
  
  pedestrianDetected = 0
  for coordIndex = 0:len
    if(lidMatrix(coordIndex) < threshold)
      pedestrianDetected = 1
    end
  end
end

  
  