using Docker.DotNet.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using vBenchSLAM.Addins;
using vBenchSLAM.Addins.ExtensionMethods;
using vBenchSLAM.Core.MapParser;
using vBenchSLAM.Core.Mappers.Abstract;
using vBenchSLAM.Core.Mappers.Base;
using vBenchSLAM.Core.Model;
using vBenchSLAM.Core.SystemMonitor;

namespace vBenchSLAM.Core.Mappers
{
    internal class OpenVslamMapper : BaseMapper, IMapper
    {
        private readonly IDatasetService _datasetService;
        
        public const string ViewerContainerImage = "openvslam-pagolin";
        public MapperType MapperType => MapperType.OpenVslam;
        public string MapFileName => "map.msg";

        public OpenVslamMapper(ProcessRunner.ProcessRunner processRunner, IDatasetService datasetService, ILogger logger) : base(processRunner, logger)
        {
            _datasetService = datasetService;
            Parser = new OpenVslamParser();
        }

        public async Task<bool> Map()
        {
            var retVal = true;
            ContainerListResponse mapperContainer = null;
            DateTime startedTime = default, finishedTime = default;
            string resourceUsageFileName = string.Empty;
            try
            {
                await ProcessRunner.EnablePangolinViewer();
                mapperContainer = await PrepareAndStartContainer();

                var statParams = new ContainerStatsParameters()
                {
                    Stream = true
                };
                startedTime = DateTime.Now;
                resourceUsageFileName = startedTime.FormatAsFileNameCode() + ".csv";
                var reporter = new SystemResourceMonitor(resourceUsageFileName, ProcessRunner, Logger);
                bool started = await DockerManager.StartContainerAsync(mapperContainer.ID);

                var attachParams = new ContainerAttachParameters()
                {
                    Stderr = true,
                    Stdout = true,
                    Stream = true
                };
#pragma warning disable 4014
                // we disable the warning because the container stats are supposed to run parallel to the container execution,
                // which we await later
                DockerManager.Client.Containers.GetContainerStatsAsync(mapperContainer.ID, statParams, reporter);
#pragma warning restore 4014
                var token = new CancellationTokenSource();
                using (var stream =
                    await DockerManager.Client.Containers.AttachContainerAsync(mapperContainer.ID, true, attachParams))
                {
                    var output = await stream.ReadOutputToEndAsync(token.Token);
                    Console.Write(output);
                }

                var exited = await DockerManager.Client.Containers.WaitContainerAsync(mapperContainer.ID);
                Logger.Information("Mapping has finished");
                finishedTime = DateTime.Now;
                retVal &= exited.StatusCode == 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong while running the algorithm");
                retVal = false;
            }
            finally
            {
                //TODO: maybe also remove the container in the parallel function
                retVal &= await ParallelStopContainersAsync(ViewerContainerImage);
                if (mapperContainer is not null)
                {
                    await DockerManager.StopContainerAsync(mapperContainer.ID);
                    await DockerManager.Client.Containers.RemoveContainerAsync(mapperContainer.ID,
                        new ContainerRemoveParameters());
                }
                
                SaveMapAndStatistics(startedTime, finishedTime, resourceUsageFileName);
            }

            return retVal;
        }

        private async Task<ContainerListResponse> PrepareAndStartContainer()
        {
            var images = await DockerManager.Client.Images.ListImagesAsync(new());
            var mapperImage = images
                .FirstOrDefault(i => i.RepoTags[0] == GetFullImageName(ViewerContainerImage));

            if (mapperImage is null) // image is not present on the users machine
            {
                await DockerManager.PullImageAsync(GetFullImageName(ViewerContainerImage));
            }

            var cfg = new Config()
            {
                Cmd = new List<string>
                {
                    GetContainerCommand()
                },
                Image = GetFullImageName(ViewerContainerImage),
                AttachStderr = true,
                AttachStdout = true,
                Env = new List<string>
                {
                    $"DISPLAY=unix{Environment.GetEnvironmentVariable("DISPLAY")}",
                    "NVIDIA_DRIVER_CAPABILITIES=all"
                }
            };
            var createParams = new CreateContainerParameters(cfg)
            {
                HostConfig = PrepareHostConfig(),
                Name = "openvlsam_pagolin"
            };
            CreateContainerResponse res = await DockerManager.Client.Containers.CreateContainerAsync(createParams);

            return await DockerManager.GetContainerByIdAsync(res.ID);
        }
        private HostConfig PrepareHostConfig()
        {
            return new HostConfig()
            {
                Binds = new List<string>
                {
                    $"/tmp/.X11-unix:/tmp/.X11-unix:rw",
                    $"{DirectoryHelper.GetDataFolderPath()}:/openvslam/build/data"
                },
                IpcMode = "host",
                NetworkMode = "host",
                Privileged = true,
                DeviceRequests = new List<DeviceRequest>
                {
                    new DeviceRequest
                    {
                        Driver = "nvidia", Capabilities = new List<IList<string>>
                        {
                            new List<string> {"compute", "compat32", "graphics", "utility", "video", "display"}
                        },
                        Count = 1
                    }
                }
            };
        }
        
        public override string GetContainerCommand()
        {
            string command = _datasetService.DatasetType == DatasetType.Kitty 
                ? $"./run_kitti_slam -v data/orb_vocab_openvslam.dbow2 -d data/sequence -c data/config_openvslam.yaml --auto-term  --map-db data/{MapFileName}" 
                : $"./run_image_slam -v data/orb_vocab_openvslam.dbow2 -c data/config_openvslam.yaml -i data/sequence --auto-term --no-sleep --map-db data/{MapFileName}";
                
            return command;
        }

        public override DatasetCheckResult ValidateDatasetCompleteness(RunnerParameters parameters)
        {
            var checkResult = _datasetService.ValidateDatasetCompleteness(parameters);
            Logger.Information("Copying the files to temporary directory");
            CopyToTemporaryFilesFolder(checkResult.GetAllFiles().ToArray());
            Logger.Information("Copying the sequence to temporary directory");
            CopySequenceFolder(checkResult.SequenceDirectory);
            Logger.Information("Files copied");
            
            return checkResult;
        }

        public void CopyMapToOutputFolder(string outputFolder)
        {
            CopyMapToOutputFolder(outputFolder, MapFileName);
        }
    }
}