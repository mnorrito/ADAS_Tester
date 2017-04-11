function [object] = plotLidar(lidarData,offset,flip)
    rgbHue = [0 1 0];
    viewSpan = inf;
    quantizationStep = 1000;
    size(lidarData);
    x=lidarData(1,:);
    y=lidarData(2,:);
    z=lidarData(3,:);
    x = x.*flip(1);
    y = y.*flip(2);
    z = z.*flip(3);

    r=lidarData(4,:);
    object = plot3(x,y,z,'.','MarkerSize',1000/quantizationStep/min(viewSpan,100),'Color',rgbHue);
    %object=trisurf(x,y,z,'.','MarkerSize',1000/quantizationStep/min(viewSpan,100),'Color',rgbHue);
    
end
