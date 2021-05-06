using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using vBenchSLAM.Addins;
using vBenchSLAM.Addins.Events;
using vBenchSLAM.Core.SystemMonitor;
using vBenchSLAM.Core.ProcessRunner;

namespace vBenchSLAM.Core.DockerCore
{
    internal class DockerManager : IDockerManager, IDisposable
    {
        /// <summary>
        /// <inheritdoc cref="IDockerManager.Client"/>
        /// </summary>
        public IDockerClient Client { get; }
        /// <summary>
        /// Process runner instance
        /// </summary>
        private readonly IProcessRunner _runner;

        public DockerManager(IProcessRunner runner)
        {
            Client = new DockerClientConfiguration().CreateClient();
            _runner = runner;
        }
        /// <summary>
        /// <inheritdoc cref="IDockerManager.ListContainersAsync"/>
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IList<ContainerListResponse>> ListContainersAsync()
        {
            IList<ContainerListResponse> containers = await Client.Containers.ListContainersAsync(
                new ContainersListParameters()
                {
                    All = true
                });
            return containers;
        }
        /// <summary>
        /// <inheritdoc cref="IDockerManager.StartContainerAsync"/>
        /// </summary>
        /// <param name="container"></param>
        /// <param name="cmdArgs"></param>
        /// <returns></returns>
        public virtual async Task<bool> StartContainerAsync(string container, string cmdArgs = "")
        {
            var parameters = new ContainerStartParameters()
            {
                DetachKeys = cmdArgs
            };
            var success = await Client.Containers.StartContainerAsync(container, parameters);

            return success;
        }
        /// <summary>
        /// <inheritdoc cref="IDockerManager.StartContainerViaCommandLineAsync"/>
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="startParameters"></param>
        /// <param name="containerCommand"></param>
        /// <returns></returns>
        [Obsolete]
        public async Task<bool> StartContainerViaCommandLineAsync(string containerName, string startParameters = "", string containerCommand = "")
        {
            await _runner.StartContainerViaCommandLineAsync(containerName, startParameters, containerCommand);
            //Client.Containers.GetContainerStatsAsync()
            //TODO: return real value
            return true;
        }
        /// <summary>
        /// <inheritdoc cref="IDockerManager.StopContainerAsync"/>
        /// </summary>
        /// <param name="containerId"></param>
        /// <returns></returns>
        public virtual async Task<bool> StopContainerAsync(string containerId)
        {
            var parameters = new ContainerStopParameters()
            {
                WaitBeforeKillSeconds = 5
            };
            var success = await Client.Containers.StopContainerAsync(containerId, parameters);
            return success;
        }
        /// <summary>
        /// <inheritdoc cref="IDockerManager.SendCommandAsync"/>
        /// </summary>
        /// <param name="containerId"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual async Task<bool> SendCommandAsync(string containerId, string command)
        {
            var container = await GetContainerByIdAsync(containerId);
            container.Command = command;

            await _runner.SendCommandToContainerAsync(containerId, command);

            return true;
        }
        /// <summary>
        /// <inheritdoc cref="IDockerManager.PullImageAsync"/>
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task PullImageAsync(string image)
        {
            await _runner.PullContainerAsync(image);
        }
        /// <summary>
        /// <inheritdoc cref="IDockerManager.GetContainerByNameAsync"/>
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public virtual async Task<ContainerListResponse> GetContainerByNameAsync(string containerName)
        {
            var containers = await ListContainersAsync();

            var cont = containers.SingleOrDefault(c => c.Image == containerName);

            return cont;
        }
        /// <summary>
        /// <inheritdoc cref="IDockerManager.GetContainerByIdAsync"/>
        /// </summary>
        /// <param name="containerId"></param>
        /// <returns></returns>
        public virtual async Task<ContainerListResponse> GetContainerByIdAsync(string containerId)
        {
            var containers = await ListContainersAsync();

            var cont = containers.SingleOrDefault(c => c.ID == containerId);

            return cont;
        }
        /// <summary>
        /// <inheritdoc cref="IDockerManager.DownloadAndBuildContainerAsync"/>
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public virtual async Task<ContainerListResponse> DownloadAndBuildContainerAsync(string repository,
            string containerName)
        {
            var containerInfo = $"{repository}:{containerName}";

            var pullExitCode = await _runner.PullContainerAsync(containerInfo);

            var buildExitCode = await _runner.BuildImageAsync(containerInfo);

            var image = await GetContainerByNameAsync(containerInfo);

            return image;
        }
        /// <summary>
        /// Disposes the <see cref="DockerManager"/> instance
        /// </summary>
        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}