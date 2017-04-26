def detect_pedestrian(distanceToWalker, throttle, speed):
    pedestrian = 0.0;
    if distanceToWalker < 18:
        throttle = -1.0
        pedestrian = 1.0
        print("/!\\ PEDESTRIAN DETECTED /!\\")
        if speed < 1:
            throttle = 0
    return throttle, pedestrian
