def detect_pedestrian(distanceToWalker):
    pedestrian = 0.0;
    if distanceToWalker < 18:
        pedestrian = 1.0
        print("/!\\ PEDESTRIAN DETECTED /!\\")
    return pedestrian
