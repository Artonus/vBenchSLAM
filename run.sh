#!/bin/bash
echo 'docker run --rm -it --net=host --gpus all -v /home/bartek/Works/vBenchSLAM/Samples:/openvslam/build/samples openvslam-socket:latest "./run_video_slam -v samples/orb_vocab/orb_vocab.dbow2 -c samples/config.yaml -m samples/video.mp4 --auto-term --no-sleep --map-db samples/generated/aist_living_lab_1_map.msg""'
docker run --rm -it --net=host --gpus all -v /home/bartek/Works/vBenchSLAM/Samples:/openvslam/build/samples openvslam-socket:latest "./run_video_slam -v samples/orb_vocab/orb_vocab.dbow2 -c samples/config.yaml -m samples/video.mp4 --auto-term --no-sleep --map-db samples/generated/aist_living_lab_1_map.msg"
#eval 
