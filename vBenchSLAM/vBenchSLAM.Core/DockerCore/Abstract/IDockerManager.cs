using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet.Models;

namespace vBenchSLAM.Core.DockerCore
{
    public interface IDockerManager
    {
        Task<IList<ContainerListResponse>> ListContainersAsync();
        Task<bool> StartContainerAsync(string containerId);
        Task<bool> StopContainerAsync(string containerId);
        void SendCommand(string containerId, string command);
        Task<string> GetContainerIdByNameAsync(string containerName);
    }
}
