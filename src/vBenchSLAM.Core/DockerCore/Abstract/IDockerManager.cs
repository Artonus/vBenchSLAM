using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace vBenchSLAM.Core.DockerCore
{
    public interface IDockerManager
    {
        /// <summary>
        /// Docker client instance
        /// </summary>
        IDockerClient Client { get; }
        /// <summary>
        /// Asynchronously list all the containers of the machine
        /// </summary>
        /// <returns></returns>
        Task<IList<ContainerListResponse>> ListContainersAsync();
        /// <summary>
        /// Asynchronously start the container
        /// </summary>
        /// <returns></returns>
        Task<bool> StartContainerAsync(string containerId, string cmdArgs = "");
        /// <summary>
        /// Asynchronously start the container using the command line process
        /// </summary>
        /// <returns></returns>
        Task<bool> StartContainerViaCommandLineAsync(string containerName, string startParameters = "", string containerCommand = "");
        /// <summary>
        /// Asynchronously stop the container
        /// </summary>
        /// <returns></returns>
        Task<bool> StopContainerAsync(string containerId);
        /// <summary>
        /// Asynchronously send command to the container
        /// </summary>
        /// <returns></returns>
        Task<bool> SendCommandAsync(string containerId, string command);
        /// <summary>
        /// Asynchronously pull image from the repository 
        /// </summary>
        /// <returns></returns>
        Task PullImageAsync(string image); 
        /// <summary>
        /// Asynchronously get the container data by a container's name
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        Task<ContainerListResponse> GetContainerByNameAsync(string containerName);
        /// <summary>
        /// Asynchronously get container by it's ID
        /// </summary>
        /// <param name="containerId"></param>
        /// <returns></returns>
        Task<ContainerListResponse> GetContainerByIdAsync(string containerId);
        /// <summary>
        /// Asynchronously download and build container
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        Task<ContainerListResponse> DownloadAndBuildContainerAsync(string repository, string containerName);
    }
}
