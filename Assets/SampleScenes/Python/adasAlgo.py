def detect_pedestrian(distanceToWalker, throttle, speed):
    pedestrian = 0.0;
    if distanceToWalker < 15:
        throttle = -1.0
        pedestrian = 1.0
        if speed < 1:
            throttle = 0
    return throttle, pedestrian
