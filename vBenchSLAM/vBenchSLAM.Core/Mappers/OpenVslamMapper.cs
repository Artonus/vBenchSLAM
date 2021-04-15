using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Serilog;
using vBenchSLAM.Addins;
using vBenchSLAM.Addins.ExtensionMethods;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.Enums;
using vBenchSLAM.Core.MapParser;
using vBenchSLAM.Core.MapParser.Models;
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
        public MapperType MapperType => MapperType.OpenVslam;
        public string MapFileName => "map.msg";

        public OpenVslamMapper(ProcessRunner.ProcessRunner processRunner, ILogger logger) : base(processRunner, logger)
        {
        }

        public async Task<bool> Map()
        {
            var retVal = true;
            ContainerListResponse socketContainer = null, serverContainer = null;
            DateTime startedTime = DateTime.Now, finishedTime = default;

            string resourceUsageFileName = startedTime.FormatAsFileNameCode() + ".csv";
            //TODO: cleanup
            try
            {
                var images = await DockerManager.Client.Images.ListImagesAsync(new());
                var viewerImage = images
                    .FirstOrDefault(i => i.RepoTags[0] == GetFullImageName(ViewerContainerImage));
                if (viewerImage is null) //image is not present at the current user's machine
                {
                    await DockerManager.PullImageAsync(GetFullImageName(ViewerContainerImage));
                }

                var serverImage = images
                    .FirstOrDefault(i => i.RepoTags[0] == GetFullImageName(ServerContainerImage));
                if (serverImage is null) //image is not present at the current user's machine
                {
                    await DockerManager.PullImageAsync(GetFullImageName(ServerContainerImage));
                }

                var srvHost = new HostConfig()
                {
                    NetworkMode = "host",
                    AutoRemove = true
                };
                var srvCfg = new Config()
                {
                    Image = GetFullImageName(ServerContainerImage)
                };
                var srvCreateParams = new CreateContainerParameters(srvCfg)
                {
                    HostConfig = srvHost,
                    Name = "openvslam_server"
                };


                // var started = await DockerManager.StartContainerViaCommandLineAsync(
                //     GetFullImageName(ServerContainerImage), "--rm -d --net=host");
                CreateContainerResponse srvRes =
                    await DockerManager.Client.Containers.CreateContainerAsync(srvCreateParams);
                socketContainer = await DockerManager.GetContainerByIdAsync(srvRes.ID);

                var started = await DockerManager.StartContainerAsync(socketContainer.ID);
                serverContainer = await DockerManager.GetContainerByNameAsync(GetFullImageName(ServerContainerImage));
                StartViewerWindow();

                var command = GetContainerCommand();
                var host = new HostConfig()
                {
                    Binds = new List<string>
                    {
                        $"{DirectoryHelper.GetDataFolderPath()}:/openvslam/build/data"
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
                var createParams = new CreateContainerParameters(config)
                {
                    HostConfig = host,
                    Name = "openvslam_viewer"
                };
                CreateContainerResponse res = await DockerManager.Client.Containers.CreateContainerAsync(createParams);
                socketContainer = await DockerManager.GetContainerByIdAsync(res.ID);
                var statParams = new ContainerStatsParameters()
                {
                    Stream = true
                };
                var reporter = new SystemResourceMonitor(resourceUsageFileName, Logger);

                started &= await DockerManager.StartContainerAsync(socketContainer.ID);
                var attachParams = new ContainerAttachParameters()
                {
                    Stderr = true,
                    Stdout = true,
                    Stream = true
                };
#pragma warning disable 4014
                // we disable the warning because the container stats are supposed to run parallel to the container execution,
                // which we await later
                DockerManager.Client.Containers.GetContainerStatsAsync(socketContainer.ID, statParams, reporter);
#pragma warning restore 4014
                var token = new CancellationTokenSource();
                using (var stream =
                    await DockerManager.Client.Containers.AttachContainerAsync(socketContainer.ID, true, attachParams))
                {
                    var output = await stream.ReadOutputToEndAsync(token.Token);
                    Console.Write(output);
                }

                var exited = await DockerManager.Client.Containers.WaitContainerAsync(socketContainer.ID);
                finishedTime = DateTime.Now;
                retVal &= exited.StatusCode == 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong: ");
            }
            finally
            {
                //TODO: maybe also remove the container in the parallel function
                retVal &= await ParallelStopContainersAsync(ServerContainerImage);
                if (socketContainer is not null)
                {
                    await DockerManager.StopContainerAsync(socketContainer.ID);
                    await DockerManager.Client.Containers.RemoveContainerAsync(socketContainer.ID,
                        new ContainerRemoveParameters());
                }

                //TODO: memory allocation issue
                SaveMapAndStatistics(startedTime, finishedTime, resourceUsageFileName);
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

        public override string GetContainerCommand()
        {
            //todo: make program accept any sequence
            var command =
                $"./run_kitti_slam -v data/orb_vocab.dbow2 -d data/sequence -c data/config.yaml --auto-term --no-sleep --map-db data/{MapFileName}";
            return command;
        }

        public override DatasetCheckResult ValidateDatasetCompleteness(RunnerParameters parameters)
        {
            string vocabFileName = "orb_vocab.dbow2", configFileName = "config.yaml", videoFileName = "video.mp4", sequenceFolderName = "sequence";

            var allFiles = Directory.GetFiles(parameters.DatasetPath);
            var fileInfos = allFiles.Select(path => new FileInfo(path)).ToList();

            var vocabFile = fileInfos.SingleOrDefault(f => f.Extension == ".dbow2" && f.Name == vocabFileName);
            if (vocabFile is null || vocabFile.Exists == false)
            {
                return new DatasetCheckResult(false,
                    new Exception($"Cannot find the vocabulary file: {vocabFileName}"));
            }

            var configFile = fileInfos.SingleOrDefault(f => f.Extension == ".yaml" && f.Name == configFileName);
            if (configFile is null || configFile.Exists == false)
            {
                return new DatasetCheckResult(false,
                    new Exception($"Cannot find the configuration file: {configFileName}"));
            }

            var sequencePath = new DirectoryInfo(Path.Combine(parameters.DatasetPath, sequenceFolderName));
            if (sequencePath.Exists == false)
            {
                return new DatasetCheckResult(false,
                         new Exception($"Cannot find the sequence folder"));
            }

            CopyToTemporaryFilesFolder(vocabFile, configFile);
            CopySequenceFolder(sequencePath);
            
            return new DatasetCheckResult(true, null);
        }

        public void CopyMapToOutputFolder(string outputFolder)
        {
            CopyMapToOutputFolder(outputFolder, MapFileName);
        }
    }
}