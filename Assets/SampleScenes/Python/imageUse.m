function [pedestrian] = imageUse(lineNum, colNum, img)

TR_x = 0.6*colNum;
TR_y = 0.6*lineNum;
BR_x = colNum;
BR_y = lineNum;

TL_x = colNum - TR_x;
TL_y = TR_y;
BL_x = colNum - BR_x;
BL_y = BR_y;

pedestrian = 0;

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

[bboxes, scores] = detectPeopleACF(rgbMat,'Model','caltech-50x21');
%bboxes
%scores
sizes = size(bboxes);
if sizes(1) > 0
    for pedItem = 1:sizes(1)
        if scores(pedItem,1) > 70
            x_min = bboxes(pedItem,1);
            x_max = x_min + bboxes(pedItem,3);
            y = bboxes(pedItem,2) + bboxes(pedItem,4);
            
            if y > TR_y
                yR_threshold = (x_max - TR_x)*(BR_y-TR_y)/(BR_x-TR_x) + TR_y;
                if y >= (yR_threshold)
                    pedestrian = 1
                end
                
                yL_threshold = (x_min - TL_x)*(BL_y-TL_y)/(BL_x-TL_x) + TL_y;
                if y >= (yL_threshold)
                    pedestrian = 1
                end
            end
        end
    end
end
rgbMat = insertObjectAnnotation(rgbMat, 'rectangle', bboxes, 'Pedestrian Detected');
imshow(rgbMat);
end
