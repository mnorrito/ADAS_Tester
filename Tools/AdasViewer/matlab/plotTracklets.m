function listObject = plotTracklets(tracklets,frame)
    fprintf('tracklets........frame %i \n',frame)
    listObject=cell(1,1);
    for i=1:length(tracklets)
        last_frame = size(tracklets{i}.poses);
        last_frame = last_frame(1,2);
        first_frame =  tracklets{i}.first_frame;
        last_frame = first_frame + last_frame -1; 
        if (frame >= first_frame) && (frame <= last_frame)
            fprintf('object number %i %s\n',i,tracklets{i}.objectType);
            %fprintf('\t%i %i %i %i\n',i,first_frame,frame,last_frame);
            listObject{i} =  plotCube(tracklets{i},frame);
        end
    end
end