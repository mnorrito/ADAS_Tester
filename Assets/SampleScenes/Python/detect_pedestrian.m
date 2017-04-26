function [pedestrian] = detect_pedestrian(distanceToWalker)
	echo off all
  pedestrian=0;
    if (distanceToWalker < 25)
        pedestrian = 1
    end
end
