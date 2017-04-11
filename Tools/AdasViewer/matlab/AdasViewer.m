function AdasViewer (dataBaseFolder,varargin)


LoopInf = 0;
enableTracklet = 0;
enOxts = 0;
%Variable Argument
fprintf(['Number of arguments: %d\n' nargin])
nVarargs = length(varargin)
fprintf('Inputs in varargin(%d):\n',nVarargs)
for k = 1:nVarargs
    fprintf('   %s %d\n', varargin{k}{1} , varargin{k}{2})
    if strcmp(varargin{k}{1},'loop')
        LoopInf = varargin{k}{2};
    end
    if strcmp(varargin{k}{1},'enTracklet')
       enableTracklet = varargin{k}{2};
    end  
    if strcmp(varargin{k}{1},'enOxts')
       enOxts = varargin{k}{2};
    end  
    
    
end


%main Folder
%global parameter
rootFolder = dataBaseFolder{1};
lidarFolder = strcat(rootFolder,'/velodyne_points/data/');
trackletsFolder = rootFolder;
listLidarFileBin=dir(fullfile(strcat(lidarFolder,'*.bin')));
listLidarFileTxt=dir(fullfile(strcat(lidarFolder,'*.txt')));
listBin = size(listLidarFileBin);
listBin = listBin(1,1);
if listBin==0
   listLidarFile= listLidarFileTxt
else 
   listLidarFile= listLidarFileBin
end
nbFrame = size(listLidarFile);
nbFrame = nbFrame(1,1);

if enableTracklet==1
    trackletsFile = strcat(rootFolder,'tracklet_labels.xml');
    fprintf('Load %s /n',trackletsFile);
    tracklets = readTracklets(trackletsFile);
end

if enOxts==1
    oxtsFile = strcat(rootFolder,'oxts/data/',fileName,'.txt');
end


%load lidar data
%
fullLidarData=cell(nbFrame,1);
for i=1:nbFrame
    fileName=listLidarFile(i).name;
    lidarFile = strcat(rootFolder,'velodyne_points/data/',fileName);
    fprintf('Load %s \n',lidarFile);
    fullLidarData{i}=loadLidarData(listBin,lidarFile);
end
%figure
fclose('all')
    f = figure();  
    whitebg(f,'k');
    view([137 32]);
    xlabel('X');
    ylabel('Y');
    zlabel('Z');
    pbaspect([10 3 1]);
    grid on;
    grid minor;
    hold on; 
   
    i=1;
    endLoop=1;
    while(endLoop  )
        currentFrame = i - 1;
        origin = plot3(0,0,0,'.','MarkerSize',25,'Color',[1 0 0]);
        lidarGraph =  plotLidar(fullLidarData{i},[0 0 0],[1 1 1]);
        if enableTracklet==1
            objectGraph = plotTracklets(tracklets,currentFrame);
        end
        title(currentFrame);    
        pause(1);
        delete(origin);
        delete(lidarGraph);
        if enableTracklet==1
        for z = 1 : length(objectGraph)
            delete(objectGraph{z});
        end
        end
        i=i+1;
        if i>nbFrame-1
            i = 1;
        end
        if i ==1 
            if LoopInf==0
                endLoop=0;
            end
        end
    end
