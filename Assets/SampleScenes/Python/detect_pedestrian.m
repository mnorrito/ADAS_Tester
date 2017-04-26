function [throttle] = detect_pedestrian(distanceToWalker, throttle, speed)
	echo off all
    if (distanceToWalker < 25)
        throttle = -0.8
        if (speed < 4)
            throttle = 0.0
        end
    end
end
