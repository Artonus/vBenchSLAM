using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace vBenchSLAM.Core.DockerCore
{
    public class DockerManager : IDockerManager, IDisposable
    {
        private readonly IDockerClient _client;
        private readonly ProcessRunner _runner;

        public DockerManager()
        {
            var uri = GetWslUri();
            
            _client = Settings.IsWsl ? new DockerClientConfiguration(uri).CreateClient() : new DockerClientConfiguration().CreateClient();
            _runner = new ProcessRunner();
        }

        private static Uri GetWslUri()
        {
            return new Uri($"tcp://127.0.0.1:{Settings.DockerWslPort}");
        }

        public async Task<IList<ContainerListResponse>> ListContainersAsync()
        {
            IList<ContainerListResponse> containers = await _client.Containers.ListContainersAsync(
            new ContainersListParameters()
            {
                All = true
            });
            return containers;
        }

        public async Task<bool> StartContainerAsync(string container, string cmdArgs = "")
        {
            var parameters = new ContainerStartParameters()
            {
                DetachKeys = cmdArgs
            };
            var success = await _client.Containers.StartContainerAsync(container, parameters);
            
            return success;
        }

        public async Task<bool> StopContainerAsync(string containerId)
        {
            var parameters = new ContainerStopParameters()
            {
                WaitBeforeKillSeconds = 10
            };
            var success = await _client.Containers.StopContainerAsync(containerId, parameters);
            return success;
        }

        public async Task<bool> SendCommandAsync(string containerId, string command)
        {
            var container = await GetContainerByIdAsync(containerId);
            container.Command = command;
            
            await _runner.SendCommandToContainerAsync(containerId, command);
            
            return true;
        }

        public async Task<ContainerListResponse> GetContainerByNameAsync(string containerName)
        {
            var containers = await ListContainersAsync();

            var cont = containers.FirstOrDefault(c => c.Image == containerName);

            return cont;
        }

        private async Task<ContainerListResponse> GetContainerByIdAsync(string containerId)
        {
            var containers = await ListContainersAsync();

            var cont = containers.FirstOrDefault(c => c.ID == containerId);

            return cont;
        }

        public async Task<ContainerListResponse> DownloadAndBuildContainer(string repository, string containerName)
        {
            var containerInfo = $"{repository}:{containerName}";

            var exitCode = await _runner.PullContainer(containerInfo);

            // if (exitCode != 1)
            //     throw new FailedToPullImageException("Unable to pull image", exitCode, containerInfo);
            
            var buildExitCode = await _runner.BuildImage(containerInfo);
            // if (buildExitCode != 1)
            //     throw new FailedToBuildImageException("Unable to build image", exitCode, containerInfo);

            var image = await GetContainerByNameAsync(containerInfo);
            
            return image;
        }
        
        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
