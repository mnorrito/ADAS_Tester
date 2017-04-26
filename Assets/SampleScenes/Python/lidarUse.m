function [pedestrian] = lidarUse(lidMatrix)
	echo off all
  len = length(lidMatrix);
  beamCut = 0;
  pedestrian=0;
  
  pedestrianDetected = 0;
  for coordIndex = 1:len/4
    beamLength = sqrt(lidMatrix(4*(coordIndex-1)+1)^2 + lidMatrix(4*(coordIndex-1)+2)^2);
    if(beamLength<20)
      beamCut=beamCut+1;
      if((4*(coordIndex)+1) <= len)
        beamLength = sqrt(lidMatrix(4*(coordIndex)+1)^2 + lidMatrix(4*(coordIndex)+2)^2);
      end
      if(beamLength<20)
        beamCut=beamCut+1;
        pedestrian = 1
        break
      end
    end
  end
end

  
  