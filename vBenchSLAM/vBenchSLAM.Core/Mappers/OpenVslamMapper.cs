using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using vBenchSLAM.Addins.Exceptions;
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
            return Run().Result;
        }

        private async Task<bool> Run()
        {
            var serverContainer =
                await _dockerManager.GetContainerByNameAsync(GetFullImageName(ServerContainerImage));
            if (serverContainer is null)
            {
                serverContainer =
                    await _dockerManager.DownloadAndBuildContainer(Settings.VBenchSLAMRepositoryName,
                        ServerContainerImage);
            }

            var socketContainer =
                await _dockerManager.GetContainerByNameAsync(GetFullImageName(ViewerContainerImage));
            if (socketContainer is null)
            {
                socketContainer =
                    await _dockerManager.DownloadAndBuildContainer(Settings.VBenchSLAMRepositoryName,
                        ViewerContainerImage);
                if (socketContainer is null)
                {
                    throw new MapperImageNotFoundException(ViewerContainerImage, "Unable to locate the image");
                }
            }

            var retVal = true;

            // if (socketContainer.Mounts.Count == 0)
            // {
            //     socketContainer.Mounts.Add(new MountPoint
            //     {
            //         Source = "/home/Bartek/Works/vBenchSLAM/Samples", //TODO: temporary folder path
            //         Destination = "/openvslam/build/samples",
            //         RW = true
            //     });
            // }

            var started = await _dockerManager.StartContainerAsync(serverContainer.ID, "-it --net=host");

            var startedNew = await _dockerManager.StartContainerAsync(socketContainer.ID/*,
                "-it --net=host --gpus all -v /home/Bartek/Works/vBenchSLAM/Samples:/openvslam/build/samples"*/);

            if (started == false)
                return false;

            StartViewerWindow();
            var command = PrepareStartCommand();
            // execution of the vslam algorithm
            retVal &= await _dockerManager.SendCommandAsync(socketContainer.ID, command);

            retVal &= await ParallelStopContainersAsync(ServerContainerImage, ViewerContainerImage);

            return retVal;
        }

        private async Task<bool> ParallelStopContainersAsync(params string[] containerNames)
        {
            var stopped = new List<Task<bool>>();
            foreach (var container in containerNames)
            {
                stopped.Add(FindAndStopContainerAsync(container));
            }

            var results = await Task.WhenAll(stopped);
            return results.All(r => r);
        }

        private async Task<bool> FindAndStopContainerAsync(string containerName)
        {
            var container = await _dockerManager.GetContainerByNameAsync(containerName);
            return await _dockerManager.StopContainerAsync(container.ID);
        }

        private void StartViewerWindow()
        {
            var url = @"http://localhost:3001";
            try
            {
                Process.Start(url);
            }
            catch (Exception)
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") {CreateNoWindow = true});
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    Process.Start("xdg-open", url);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    Process.Start("open", url);
                else
                    throw;
            }
        }

        public bool Stop()
        {
            throw new NotImplementedException();
        }

        public string PrepareStartCommand()
        {
            //TODO: create temporary folder to store data to run
            var command =
                @"""./run_video_slam -v samples/orb_vocab/orb_vocab.dbow2 -c samples/config.yaml -m samples/video.mp4 " +
                @"--auto-term --no-sleep --map-db samples/generated/aist_living_lab_1_map.msg --debug && exit""";
            Console.WriteLine(command);
            return command;
        }
    }
}