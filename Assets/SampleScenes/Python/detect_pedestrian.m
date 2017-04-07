function [a, b] = detect_pedestrian(distanceToWalker, throttle, speed)
    pedestrian = 0.0
    
    if (distanceToWalker < 18)
        throttle = -1.0
        pedestrian = 1.0
        if (speed < 1.5)
            throttle = 0.0
        end
    end
    a = throttle
    b = pedestrian
end
