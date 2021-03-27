using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.Enums;
using vBenchSLAM.Core.Mappers.Abstract;
using vBenchSLAM.Core.Mappers.Base;
using vBenchSLAM.Core.Model;
using vBenchSLAM.Core.SystemMonitor;

namespace vBenchSLAM.Core.Mappers
{
    public class OpenVslamMapper : BaseMapper, IMapper
    {
        public const string ServerContainerImage = "openvslam-server";
        public const string ViewerContainerImage = "openvslam-socket";
        private const string FullName = "";

        public MapperTypeEnum MapperType => MapperTypeEnum.OpenVslam;
        public string FullFrameworkName => FullName;

        public OpenVslamMapper(IDockerManager dockerManager) : base(dockerManager)
        {
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
            var retVal = true;
            ContainerListResponse socketContainer = null;
            try
            {
                var command = GetContainerCommand();
                var host = new HostConfig()
                {
                    Binds = new List<string>
                    {
                        "/home/bartek/Works/vBenchSLAM/Samples:/openvslam/build/samples"
                    },
                    NetworkMode = "host"
                    //AutoRemove = true
                };
                var config = new Config()
                {
                    Cmd = new List<string>()
                    {
                        command
                    },
                    Image = GetFullImageName(ViewerContainerImage),
                    AttachStderr = true,
                    AttachStdout = true,
                };

                var started = await DockerManager.StartContainerViaCommandLineAsync(
                    GetFullImageName(ServerContainerImage),
                    "--rm -d --net=host");

                var serverContainer =
                    await DockerManager.GetContainerByNameAsync(GetFullImageName(ServerContainerImage));

                if (started == false)
                    return false;

                StartViewerWindow();
                var createParams = new CreateContainerParameters(config)
                {
                    HostConfig = host,

                };

                var res = await DockerManager.Client.Containers.CreateContainerAsync(createParams);
                
                var statParams = new ContainerStatsParameters()
                {
                    Stream = true
                };
                var reporter = new SystemResource();
                socketContainer = await DockerManager.GetContainerByIdAsync(res.ID);



                started &= await DockerManager.StartContainerAsync(socketContainer.ID);
                var attachParams = new ContainerAttachParameters()
                {
                    Stderr = true,
                    Stdout = true,
                    Stream = true
                };
                DockerManager.Client.Containers.GetContainerStatsAsync(socketContainer.ID, statParams, reporter);
                var token = new CancellationTokenSource();
                using (var stream =
                    await DockerManager.Client.Containers.AttachContainerAsync(socketContainer.ID, true, attachParams))
                {
                    var output = await stream.ReadOutputToEndAsync(token.Token);
                    Console.Write(output);
                }                
                var exited = await DockerManager.Client.Containers.WaitContainerAsync(socketContainer.ID);
                retVal &= exited.StatusCode == 0;                
            }
            finally
            {
                retVal &= await ParallelStopContainersAsync(ServerContainerImage);
                if (socketContainer is not null)
                {
                    await DockerManager.StopContainerAsync(socketContainer.ID);
                    await DockerManager.Client.Containers.RemoveContainerAsync(socketContainer.ID, new ContainerRemoveParameters());
                }
            }

            return retVal;
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

        public string GetContainerCommand()
        {
            //TODO: create temporary folder to store data to run
            var command =
                "./run_video_slam -v samples/orb_vocab/orb_vocab.dbow2 -c samples/config.yaml -m samples/video.mp4 --auto-term --no-sleep --map-db samples/map.msg";
            Console.WriteLine(command);
            return command;
        }
    }
}