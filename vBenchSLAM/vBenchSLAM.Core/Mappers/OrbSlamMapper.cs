using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Serilog;
using vBenchSLAM.Addins;
using vBenchSLAM.Addins.ExtensionMethods;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.Enums;
using vBenchSLAM.Core.Mappers.Abstract;
using vBenchSLAM.Core.Mappers.Base;
using vBenchSLAM.Core.Model;
using vBenchSLAM.Core.ProcessRunner;
using vBenchSLAM.Core.SystemMonitor;

namespace vBenchSLAM.Core.Mappers
{
    public class OrbSlamMapper : BaseMapper, IMapper
    {
        public const string MapperContainerImage = "orbslam2";
        public MapperType MapperType => MapperType.OrbSlam;
        public string MapFileName => "KeyFrameTrajectory.txt";

        private readonly OrbSlamProcessRunner _processRunner;
        public OrbSlamMapper(OrbSlamProcessRunner processRunner, ILogger logger) : base(processRunner, logger)
        {
            _processRunner = processRunner;
        }

        public async Task<bool> Map()
        {
            bool retVal = true;
            ContainerListResponse mapperContainer = null;
            DateTime startedTime = DateTime.Now, finishedTime = default;
            string resourceUsageFileName = startedTime.FormatAsFileNameCode() + ".csv";
            try
            {
                await _processRunner.EnablePangolinViewer();
                mapperContainer = await PrepareAndStartContainer();
                var statParams = new ContainerStatsParameters()
                {
                    Stream = true
                };

                var reporter = new SystemResourceMonitor(resourceUsageFileName, Logger);
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
                if (mapperContainer is not null)
                {
                    await DockerManager.StopContainerAsync(mapperContainer.ID);
                    await DockerManager.Client.Containers.RemoveContainerAsync(mapperContainer.ID,
                        new ContainerRemoveParameters());
                }
                //TODO: validate the map
                SaveMapAndStatistics(startedTime, finishedTime, resourceUsageFileName);
            }

            return retVal;
        }

        private async Task<ContainerListResponse> PrepareAndStartContainer()
        {
            var images = await DockerManager.Client.Images.ListImagesAsync(new());
            var mapperImage = images
                .FirstOrDefault(i => i.RepoTags[0] == GetFullImageName(MapperContainerImage));

            if (mapperImage is not null) // image is not present on the users machine
            {
                await DockerManager.PullImageAsync(GetFullImageName(MapperContainerImage));
            }

            var cfg = new Config()
            {
                Cmd = new List<string>
                {
                    GetContainerCommand()
                },
                Image = GetFullImageName(MapperContainerImage),
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
                Name = "orb_slam2"
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
                    $"{DirectoryHelper.GetDataFolderPath()}:/home/ORB_SLAM2/data"
                },
                IpcMode = "host",
                NetworkMode = "host",
                Privileged = true,
                DeviceRequests = new List<DeviceRequest>
                {
                    new DeviceRequest
                    {
                        //TODO: check for the correct data
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
            string command =
                $"./Examples/Monocular/mono_kitti data/ORBvoc.txt data/config-orb.yaml data/sequence " +
                $"&& cp {MapFileName} data/{MapFileName}";
            return command;
        }

        public override DatasetCheckResult ValidateDatasetCompleteness(RunnerParameters parameters)
        {
            string vocabFileName = "ORBvoc.txt", configFileName = "config-orb.yaml", sequenceFolderName = "sequence";

            var allFiles = Directory.GetFiles(parameters.DatasetPath);
            var fileInfos = allFiles.Select(path => new FileInfo(path)).ToList();

            var vocabFile = fileInfos.SingleOrDefault(f => f.Extension == ".txt" && f.Name == vocabFileName);
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