using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
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
        private const string ServerContainerImage = "openvslam-server";
        private const string ViewerContainerImage = "openvslam-socket";
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
            // var serverContainer =
            //     await _dockerManager.GetContainerByNameAsync(GetFullImageName(ServerContainerImage));
            // if (serverContainer is null)
            // {
            //     serverContainer =
            //         await _dockerManager.DownloadAndBuildContainer(Settings.VBenchSlamRepositoryName,
            //             ServerContainerImage);
            // }

            // var socketContainer =
            //     await _dockerManager.GetContainerByNameAsync(GetFullImageName(ViewerContainerImage));
            // if (socketContainer is null)
            // {
            //     socketContainer =
            //         await _dockerManager.DownloadAndBuildContainer(Settings.VBenchSlamRepositoryName,
            //             ViewerContainerImage);
            //     if (socketContainer is null)
            //     {
            //         throw new MapperImageNotFoundException(ViewerContainerImage, "Unable to locate the image");
            //     }
            // }

            // if (socketContainer.Mounts.Count == 0)
            // {
            //     socketContainer.Mounts.Add(new MountPoint
            //     {
            //         Source = "/home/Bartek/Works/vBenchSLAM/Samples", //TODO: temporary folder path
            //         Destination = "/openvslam/build/samples",
            //         RW = true
            //     });
            // }
            //ContainerListResponse socketContainer = null;
            try
            {
                var started = await DockerManager.StartContainerViaCommandLineAsync(
                    GetFullImageName(ServerContainerImage),
                    "--rm -d --net=host replaceME");

                var serverContainer =
                    await DockerManager.GetContainerByNameAsync(GetFullImageName(ServerContainerImage));

                if (started == false)
                    return false;

                StartViewerWindow();
                
                var command = PrepareStartCommand();
                
                // var config = new Config()
                // {
                //     Cmd = new List<string>()
                //     {
                //         command
                //     },
                //     Image = "openvslam-socket:latest",
                //     AttachStderr = true,
                //     AttachStdout = true,
                //     Volumes = new Dictionary<string, EmptyStruct>()
                //     {
                //         {"/home/bartek/Works/vBenchSLAM/Samples:/openvslam/build/samples", new EmptyStruct()}
                //     },
                //     
                // };
                //
                // var res = await DockerManager.Client.Containers.CreateContainerAsync(new CreateContainerParameters(config));
                //
                // socketContainer = await DockerManager.GetContainerByIdAsync(res.ID);
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
                // //var token = new CancellationTokenSource();
                // using (var stream = await DockerManager.Client.Containers.AttachContainerAsync(socketContainer.ID, true, attachParams))
                // {
                //     using (var sr = new StreamReader(stream, ))
                //     {
                //         
                //     }    
                // }
                //
                // started &= await DockerManager.StartContainerAsync(socketContainer.ID);

                #region OldStuff
                //IT'S WORKING!!!!!!!!!!!!!!
                var cmd =
                    "--rm -it --net=host --gpus all -v /home/bartek/Works/vBenchSLAM/Samples:/openvslam/build/samples " +
                    "replaceME " + command;
                await DockerManager.StartContainerViaCommandLineAsync("openvslam-socket:latest", cmd);
                var socketContainer =
                    await DockerManager.GetContainerByNameAsync(GetFullImageName(ViewerContainerImage));

                #endregion
            }
            finally
            {
                retVal &= await ParallelStopContainersAsync(ServerContainerImage);
                // if (socketContainer is not null)
                // {
                //     await DockerManager.StopContainerAsync(socketContainer.ID);
                //     var removeParams = new ContainerRemoveParameters();
                //     await DockerManager.Client.Containers.RemoveContainerAsync(socketContainer.ID, removeParams);
                // }
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