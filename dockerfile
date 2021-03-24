FROM artonus/vbenchslam:openvslam-socket as socket
WORKDIR /openvslam/build
ENTRYPOINT ["/bin/sh", "-l", "-c"]