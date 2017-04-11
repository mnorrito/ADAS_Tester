function cubeObject = plotCube(tracklet,frame)
    %box size
    l = tracklet.l;
    w = tracklet.w;
    h = tracklet.h;
    
    cube = [
        -l/2 -w/2 0;  %A
        -l/2 w/2  0;  %B
        l/2  w/2  0;  %C
        l/2  -w/2 0;  %D
        -l/2 -w/2 h;  %E
        -l/2 w/2  h;  %F
        l/2  w/2  h;  %G
        l/2  -w/2 h   %H
        ];      
    
       fac = [1 2 3 4; 
        5 6 7 8; 
        1 4 8 5; 
        1 2 6 5; 
        3 4 7 6; 
        2 3 8 7];
    
    

    cube = transpose(cube);
    nbPoses = size(tracklet.poses);
    nbPoses = nbPoses(1,2);
    firstFrame = tracklet.first_frame;
    poseIndex = frame - firstFrame + 1; 
    poseInfo = tracklet.poses(:,poseIndex);
       % 1,'tx'
       % 2,'ty'
       % 3,'tz'
       % rotation
       % 4,'rx'
       % 5,'ry'
       % 6,'rz'
       % state
       % 7,'state' 
       % occlusion
       % 8,'occlusion'
       % 9,'occlusion_kf'
       % truncation
       % 10,'truncation'
       % averaged mechanical turk
       % 11,'amt_occlusion'
       % 12,'amt_occlusion_kf'
       % 13,'amt_border_l'
       % 14,'amt_border_r'
       % 15'amt_border_kf'
        
        
    T = poseInfo(1:3);
    R = poseInfo(4:6);
    
    %Z rotation
    MOZ=[   cos(R(3)) -sin(R(3)) 0;
            sin(R(3)) cos(R(3))  0;
            0         0          1
            ];
        
    for i = 1:8
        newCube(:,i) = MOZ * cube(:,i);
        newCube(:,i) = newCube(:,i)+T;
    end
    newCube = transpose(newCube);
       
    cubeObject = patch('Faces',fac,'Vertices',newCube,'FaceColor','r');  % patch function
    
    %material shiny;
    %alpha('color');
    %alphamap('rampdown');
    

end