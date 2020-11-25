using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vBenchSLAM.Core.DockerCore
{
    public class DockerManager : IDockerManager
    {
        private readonly IDockerClient _client;

        public DockerManager()
        {
            var uri = GetWslUri();
            _client = new DockerClientConfiguration(uri).CreateClient();
        }

        private static Uri GetWslUri()
        {
            return new Uri("tcp://127.0.0.1:2375");
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

            var success = await _client.Containers.StartContainerAsync(container, null);
            return success;
        }
    }
}
