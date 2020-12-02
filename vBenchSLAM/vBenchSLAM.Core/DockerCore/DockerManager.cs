using System;
using System.Collections.Generic;
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
            if (Settings.CheckWsl())
            {
                
            }
            _client = new DockerClientConfiguration(uri).CreateClient();
        }

        private static Uri GetWslUri()
        {
            

            return new Uri($"tcp://127.0.0.1:{Settings.WslPort}");
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

        public void SaveMap()
        {
            throw new NotImplementedException();
        }
    }
}
