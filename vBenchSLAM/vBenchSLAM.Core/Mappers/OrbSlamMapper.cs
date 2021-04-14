using System;
using System.Collections.Generic;
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
using vBenchSLAM.Core.SystemMonitor;

namespace vBenchSLAM.Core.Mappers
{
    public class OrbSlamMapper : BaseMapper, IMapper
    {
        public const string MapperContainerImage = "orbslam2";
        public MapperType MapperType => MapperType.OrbSlam;
        public string MapFileName => "KeyFrameTrajectory.txt";

        public OrbSlamMapper(IDockerManager dockerManager, ILogger logger) : base(dockerManager, logger)
        {
        }

        public async Task<bool> Map()
        {
            bool retVal = true;
            ContainerListResponse mapperContainer = null;
            DateTime startedTime = DateTime.Now, finishedTime = default;
            string resourceUsageFileName = startedTime.FormatAsFileNameCode() + ".csv";
            try
            {
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
                finishedTime = DateTime.Now;
                retVal &= exited.StatusCode == 0;
                //TODO: wait for exit
                //TODO: parse created map
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
                AttachStdout = true
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
                        Driver = "nvidia", Capabilities = new List<IList<string>>
                        {
                            new List<string> {"all"}
                        },
                        Count = -1,
                        //TODO: get valid dpu id
                        DeviceIDs = new List<string> {"GPU-fef8089b-4820-abfc-e83e-94318197576e"},
                        Options = new Dictionary<string, string>
                        {
                            {"--gpus", "all"},
                        }
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
            throw new System.NotImplementedException();
        }

        public void CopyMapToOutputFolder(string outputFolder)
        {
            throw new System.NotImplementedException();
        }
    }
}