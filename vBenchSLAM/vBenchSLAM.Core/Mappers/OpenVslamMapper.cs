﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
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
                var command = PrepareStartCommand();
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

                socketContainer = await DockerManager.GetContainerByIdAsync(res.ID);
                started &= await DockerManager.StartContainerAsync(socketContainer.ID);

                var exited = await DockerManager.Client.Containers.WaitContainerAsync(socketContainer.ID);
                // if (socketContainer is null)
                // {
                //     return false;
                // }
                //
                // var attachParams = new ContainerAttachParameters()
                // {
                //     Stderr = true,
                //     Stdout = true,
                //     Stream = true
                // };
                // var token = new CancellationTokenSource();
                // using (var stream = await DockerManager.Client.Containers.AttachContainerAsync(socketContainer.ID, true, attachParams))
                // {
                //     var output = await stream.ReadOutputToEndAsync(token.Token);
                // }


                #region Old

                // var startParams =
                //     "--rm -it --net=host --gpus all -v /home/bartek/Works/vBenchSLAM/Samples:/openvslam/build/samples";
                //
                // await DockerManager.StartContainerViaCommandLineAsync(GetFullImageName(ViewerContainerImage),
                //     startParams, command);
                // var socketContainer =
                //     await DockerManager.GetContainerByNameAsync(GetFullImageName(ViewerContainerImage));

                #endregion
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

        public string PrepareStartCommand()
        {
            //TODO: create temporary folder to store data to run
            var command =
                "./run_video_slam -v samples/orb_vocab/orb_vocab.dbow2 -c samples/config.yaml -m samples/video.mp4 --auto-term --no-sleep --map-db samples/generated/aist_living_lab_1_map.msg";
            Console.WriteLine(command);
            return command;
        }
    }
}