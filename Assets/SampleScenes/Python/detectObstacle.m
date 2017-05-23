lidarPings=load('../../../2017-05-15T16.14.50/LIDAR_OUTPUT/0000002833.txt');

pts=pointCloud(lidarPings(:,[1:3]));

[plane,inliers,outliers]=pcfitplane(pts,0.3,[0 0 1]);
ptsGround=select(pts,inliers);
ptsObjs=select(pts,outliers);

loc=double(ptsObjs.Location);
x = loc(:,1); y = loc(:,2); z = loc(:,3);
[angle,d,h]=cart2pol(x,y,y);
angleDeg = angle/pi*180;
idxObstacle=find((angleDeg<10)&(angleDeg>-10)&(d<30)&(d>5)&(y>-3)&(y<3));

ptsObstacle=select(ptsObjs,idxObstacle);
ptsObstacle.Color=repmat(uint8([255 0 0]),ptsObstacle.Count,1);

figure;subplot(2,1,1);pcshow(ptsGround,'MarkerSize',20);subplot(2,1,2);pcshow(ptsObjs,'MarkerSize',20);hold on;pcshow(ptsObstacle,'MarkerSize',40);
