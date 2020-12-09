using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace vBenchSLAM.Core.DockerCore
{
    public class DockerManager : IDockerManager
    {
        private readonly IDockerClient _client;

        public DockerManager()
        {
            var uri = GetWslUri();
            
            _client = Settings.IsWsl ? new DockerClientConfiguration(uri).CreateClient() : new DockerClientConfiguration().CreateClient();
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

        public async Task<bool> StartContainerAsync(string container)
        {
            var parameters = new ContainerStartParameters();
            var success = await _client.Containers.StartContainerAsync(container, parameters);
            return success;
        }

        public async Task<bool> StopContainerAsync(string containerId)
        {
            var success = await _client.Containers.StopContainerAsync(containerId, null);
            return success;
        }

        public void SendCommand(string containerId, string command)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetContainerIdByNameAsync(string containerName)
        {
            var containers = await ListContainersAsync();

            var cont = containers.FirstOrDefault(c => c.Image == containerName);

            return cont?.ID;
        }
    }
}
