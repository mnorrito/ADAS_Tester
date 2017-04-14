function [throttle] = detect_pedestrian(distanceToWalker, throttle, speed)
	echo off all
    if (distanceToWalker < 18)
        throttle = -1.0
        if (speed < 1)
            throttle = 0.0
        end
    end
end
