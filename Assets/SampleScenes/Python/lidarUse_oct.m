function [pedestrian] = lidarUse_oct(lidMatrix)
	echo off all
  len = length(lidMatrix);
  pedestrian = 0;
  for coordIndex = 1:len/4
    beamLength = sqrt(lidMatrix(4*(coordIndex-1)+1)^2 + lidMatrix(4*(coordIndex-1)+2)^2);
    if(beamLength<20)
      if((4*(coordIndex)+1) <= len)
        beamLength = sqrt(lidMatrix(4*(coordIndex)+1)^2 + lidMatrix(4*(coordIndex)+2)^2);
      end
      if(beamLength<19)
        pedestrian = 1;
        break
      end
    end
  end
end

  
  