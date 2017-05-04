function [pedestrian] = imageUse(img)

    vect=[];
    rCol = [];
    gCol = [];
    bCol = [];
    rMat = [];
    gMat = [];
    bMat = [];
    lineNum = size(img,2);
    for ln = 1:lineNum
        colArray = img{ln};
        colNum = size(colArray,2);
        rCol = [];
        gCol = [];
        bCol = [];
        for cl = 1: colNum
            rgbArray = colArray{cl};
            rCol = [rCol rgbArray{1}];
            gCol = [gCol rgbArray{2}];
            bCol = [bCol rgbArray{3}];
        end
        if size(rMat,2) == 0
            rMat = rCol;
            gMat = gCol;
            bMat = bCol;
        else
            rMat = [rMat ; rCol];
            gMat = [gMat ; gCol];
            bMat = [bMat ; bCol];
        end
    end

    rgbMat(:,:,1) = double(rMat)/256;
    rgbMat(:,:,2) = double(gMat)/256;
    rgbMat(:,:,3) = double(bMat)/256;
    
    image(rgbMat);
end