using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace vBenchSLAM.Core.DockerCore
{
    public interface IDockerManager
    {
        IDockerClient Client { get; }
        Task<IList<ContainerListResponse>> ListContainersAsync();
        Task<bool> StartContainerAsync(string containerId, string cmdArgs = "");
        Task<bool> StartContainerViaCommandLineAsync(string containerName, string startParameters = "", string containerCommand = "");
        Task<bool> StopContainerAsync(string containerId);
        Task<bool> SendCommandAsync(string containerId, string command);
        Task<ContainerListResponse> GetContainerByNameAsync(string containerName);
        Task<ContainerListResponse> GetContainerByIdAsync(string containerId);
        Task<ContainerListResponse> DownloadAndBuildContainer(string repository, string containerName);
    }
}
