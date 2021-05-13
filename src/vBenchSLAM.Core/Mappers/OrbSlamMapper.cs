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
using vBenchSLAM.Core.MapParser;
using vBenchSLAM.Core.Mappers.Abstract;
using vBenchSLAM.Core.Mappers.Base;
using vBenchSLAM.Core.Model;
using vBenchSLAM.Core.SystemMonitor;

namespace vBenchSLAM.Core.Mappers
{
    /// <summary>
    /// Mapper for the ORB_SLAM2 framework
    /// </summary>
    internal class OrbSlamMapper : BaseMapper, IMapper
    {
        /// <summary>
        /// Dataset Service instance
        /// </summary>
        private readonly IDatasetService _datasetService;
        /// <summary>
        /// Tag of a container that contains the OpenVSLAM algorithm
        /// </summary>
        public const string MapperContainerImage = "orbslam2";
        /// <summary>
        /// <inheritdoc cref="IMapper.MapperType"/>
        /// </summary>
        public MapperType MapperType => MapperType.OrbSlam;
        /// <summary>
        /// <inheritdoc cref="IMapper.MapFileName"/>
        /// </summary>
        public string MapFileName => "KeyFrameTrajectory.txt";

        public OrbSlamMapper(ProcessRunner.ProcessRunner processRunner, IDatasetService datasetService, ILogger logger) : base(processRunner, logger)
        {
            _datasetService = datasetService;
            Parser = new OrbSlamParser();
        }
        /// <summary>
        /// <inheritdoc cref="IMapper.Map"/>
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Map()
        {
            bool retVal = true;
            ContainerListResponse mapperContainer = null;
            DateTime startedTime = default, finishedTime = default;
            string resourceUsageFileName = string.Empty;
            try
            {
                await ProcessRunner.EnablePangolinViewerAsync();
                mapperContainer = await PrepareContainer();
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
                if (mapperContainer is not null)
                {
                    await DockerManager.StopContainerAsync(mapperContainer.ID);
                    await DockerManager.Client.Containers.RemoveContainerAsync(mapperContainer.ID,
                        new ContainerRemoveParameters());
                }
                ConfirmRunFinished(startedTime, finishedTime, resourceUsageFileName);
            }

            return retVal;
        }
        /// <summary>
        /// Prepares the container to be ready to run
        /// </summary>
        /// <returns></returns>
        private async Task<ContainerListResponse> PrepareContainer()
        {
            var images = await DockerManager.Client.Images.ListImagesAsync(new());
            var mapperImage = images
                .FirstOrDefault(i => i.RepoTags[0] == GetFullImageName(MapperContainerImage));

            if (mapperImage is null) // image is not present on the user's machine
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
        /// <summary>
        /// Prepares the configuration of a host machine to be used by the current container
        /// </summary>
        /// <returns></returns>
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
                        Driver = "nvidia", Capabilities = new List<IList<string>>
                        {
                            new List<string> {"compute", "compat32", "graphics", "utility", "video", "display"}
                        },
                        Count = 1
                    }
                }
            };
        }
        /// <summary>
        /// <inheritdoc cref="BaseMapper.GetContainerCommand"/>
        /// </summary>
        /// <returns></returns>
        public override string GetContainerCommand()
        {
            string command = _datasetService.DatasetType == DatasetType.Kitty
                ? $"./Examples/Monocular/mono_kitti data/orb_vocab_orbslam2.txt data/config_orbslam2.yaml data/sequence; cp {MapFileName} data/{MapFileName}"
                : $"./Examples/Monocular/mono_kitti data/orb_vocab_orbslam2.txt data/config_orbslam2.yaml data/sequence; cp {MapFileName} data/{MapFileName}";
            return command;
        }
        /// <summary>
        /// <inheritdoc cref="IMapper.ValidateDatasetCompleteness"/>
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override DatasetCheckResult ValidateDatasetCompleteness(RunnerParameters parameters)
        {
            var checkResult = _datasetService.ValidateDatasetCompleteness(parameters);
            if (checkResult.IsValid == false)
            {
                return checkResult;
            }
            Logger.Information("Copying the files to temporary directory");
            CopyToTemporaryFilesFolder(checkResult.GetAllFiles().ToArray());
            if (_datasetService.DatasetType == DatasetType.Kitty)
            {
                Logger.Information("Copying the sequence to temporary directory");
                CopySequenceFolder(checkResult.SequenceDirectory);    
            }
            Logger.Information("Files copied");
            return checkResult;
        }
        /// <summary>
        /// <inheritdoc cref="IMapper.CopyMapToOutputFolder"/>
        /// </summary>
        /// <param name="outputFolder"></param>
        public void CopyMapToOutputFolder(string outputFolder)
        {
            CopyMapToOutputFolder(outputFolder, MapFileName);
        }
    }
}