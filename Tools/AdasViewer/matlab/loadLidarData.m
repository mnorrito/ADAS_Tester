
function [lidarData] = loadLidarData(type,file)
    quantizationStep = 1000;
    if type==0
        lidarData=load(file);
        lidarData = (round(lidarData*quantizationStep))/quantizationStep;
        %tmp=lidarData(:,1);
        %lidarData(:,1)=lidarData(:,2);
        %lidarData(:,2)=tmp;
        lidarData=transpose(lidarData);      
    else
        fid = fopen(file,'r');
        lidarData=fread(fid,[4,inf],'float');
        lidarData = (round(lidarData*quantizationStep))/quantizationStep;
    end
end

