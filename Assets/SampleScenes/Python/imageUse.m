function [pedestrian] = imageUse(lineNum, colNum, img)
    vec = img(1:3:end);
    vec = cell2mat(vec);
    rVec = reshape(vec,colNum, lineNum);
    
    vec = img(2:3:end);
    vec = cell2mat(vec);
    gVec = reshape(vec,colNum, lineNum);

    vec = img(3:3:end);
    vec = cell2mat(vec);
    bVec = reshape(vec,colNum, lineNum);

    
    rgbMat(:,:,1) = double(rVec.')/256;
    rgbMat(:,:,2) = double(gVec.')/256;
    rgbMat(:,:,3) = double(bVec.')/256;
    
    %detector = vision.PeopleDetector;
    [bboxes, scores] = detectPeopleACF(rgbMat,'Model','caltech-50x21');
    rgbMat = insertObjectAnnotation(rgbMat, 'rectangle', bboxes, 'Pedestrian Detected'); 
    imshow(rgbMat);
    %image(rgbMat);
    pedestrian = 0;
end