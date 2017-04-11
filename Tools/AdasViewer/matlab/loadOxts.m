function [oxtsData] = loadOxts(file)

oxtsDataTmp=importdata(file);
oxtsData = [];
oxtsData.lat= oxtsDataTmp(1,1);%   latitude of the oxts-unit (deg)
oxtsData.lon= oxtsDataTmp(1,2);%   longitude of the oxts-unit (deg)
oxtsData.alt= oxtsDataTmp(1,3);%   altitude of the oxts-unit (m)
oxtsData.roll= oxtsDataTmp(1,4);%  roll angle (rad),    0 = level, positive = left side up,      range: -pi   .. +pi
oxtsData.pitch= oxtsDataTmp(1,5);% pitch angle (rad),   0 = level, positive = front down,        range: -pi/2 .. +pi/2
oxtsData.yaw= oxtsDataTmp(1,6);%   heading (rad),       0 = east,  positive = counter clockwise, range: -pi   .. +pi
oxtsData.vn= oxtsDataTmp(1,7);%    velocity towards north (m/s)
oxtsData.ve = oxtsDataTmp(1,8);%   velocity towards east (m/s)
oxtsData.vf = oxtsDataTmp(1,9);%   forward velocity, i.e. parallel to earth-surface (m/s)
oxtsData.vl = oxtsDataTmp(1,10);%   leftward velocity, i.e. parallel to earth-surface (m/s)
oxtsData.vu= oxtsDataTmp(1,11);%    upward velocity, i.e. perpendicular to earth-surface (m/s)
oxtsData.ax = oxtsDataTmp(1,12);%   acceleration in x, i.e. in direction of vehicle front (m/s^2)
oxtsData.ay = oxtsDataTmp(1,13);%  acceleration in y, i.e. in direction of vehicle left (m/s^2)
oxtsData.ay = oxtsDataTmp(1,14);%   acceleration in z, i.e. in direction of vehicle top (m/s^2)
oxtsData.af  = oxtsDataTmp(1,15);%  forward acceleration (m/s^2)
oxtsData.al = oxtsDataTmp(1,16);%   leftward acceleration (m/s^2)
oxtsData.au = oxtsDataTmp(1,17);%   upward acceleration (m/s^2)
oxtsData.wx = oxtsDataTmp(1,18);%   angular rate around x (rad/s)
oxtsData.wy = oxtsDataTmp(1,19);%   angular rate around y (rad/s)
oxtsData.wz = oxtsDataTmp(1,20);%   angular rate around z (rad/s)
oxtsData.wf = oxtsDataTmp(1,21);%   angular rate around forward axis (rad/s)
oxtsData.wl = oxtsDataTmp(1,22);%   angular rate around leftward axis (rad/s)
oxtsData.wu = oxtsDataTmp(1,23);%   angular rate around upward axis (rad/s)
oxtsData.pos_accuracy = oxtsDataTmp(1,24);% velocity accuracy (north/east in m)
oxtsData.vel_accuracy = oxtsDataTmp(1,25);% velocity accuracy (north/east in m/s)
oxtsData.navstat= oxtsDataTmp(1,26);%       navigation status (see navstat_to_string)
oxtsData.numsats  = oxtsDataTmp(1,27);%     number of satellites tracked by primary GPS receiver
oxtsData.posmode   = oxtsDataTmp(1,28);%    position mode of primary GPS receiver (see gps_mode_to_string)
oxtsData.velmode   = oxtsDataTmp(1,29);%    velocity mode of primary GPS receiver (see gps_mode_to_string)
oxtsData.orimode   = oxtsDataTmp(1,30);%    orientation mode of primary GPS receiver (see gps_mode_to_string)

end
