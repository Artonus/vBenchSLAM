using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.Enums;
using vBenchSLAM.Core.Mappers.Abstract;
using vBenchSLAM.Core.Mappers.Base;

namespace vBenchSLAM.Core.Mappers
{
    public class OpenVslamMapper : BaseMapper, IMapper
    {
        private readonly IDockerManager _dockerManager;
        private const string ServerContainerImage = "openvslam-server";
        private const string ViewerContainerImage = "openvslam-socket";
        private const string FullName = "";

        public MapperTypeEnum MapperType => MapperTypeEnum.OpenVslam;
        public string FullFrameworkName => FullName;

        public OpenVslamMapper(IDockerManager dockerManager)
        {
            _dockerManager = dockerManager;
        }
        public string SaveMap()
        {
            throw new NotImplementedException();
        }

        public bool ShowMap()
        {
            throw new NotImplementedException();
        }
        public bool Start()
        {
            return StartAsync().Result;
        }

        private async Task<bool> StartAsync()
        {
            var serverContainer = await _dockerManager.GetContainerByNameAsync(ServerContainerImage);
            if (serverContainer is null)
                return false;
            var socketContainer = await _dockerManager.GetContainerByNameAsync(ViewerContainerImage);
            if (socketContainer is null)
                return false;

            socketContainer.Mounts.Add(new MountPoint
            {
                Source = string.Empty, //TODO: temporary folder path
                Destination = "/openvslam/build/samples",
                RW = true
            });

            var started = await _dockerManager.StartContainerAsync(serverContainer.ID) 
                          && await _dockerManager.StartContainerAsync(socketContainer.ID);
            if (started)
            {
                StartViewerWindow();
                var command = PrepareStartCommand();
                await _dockerManager.SendCommandAsync(serverContainer.ID, command);
            }

            var hasStarted = started;

            return hasStarted;
        }

        private void StartViewerWindow()
        {
            Process.Start("https://localhost:3001");
        }

        public bool Stop()
        {
            throw new NotImplementedException();
        }

        public string PrepareStartCommand()
        {
            //TODO: create temporary folder to store data to run
            var command =
                "./ run_video_slam -v samples/orb_vocab/orb_vocab.dbow2 -c samples/config.yaml -m samples/video.mp4 " +
                "--auto-term --no-sleep --map-db samples/generated/aist_living_lab_1_map.msg";
            return command;
        }
    }
}
