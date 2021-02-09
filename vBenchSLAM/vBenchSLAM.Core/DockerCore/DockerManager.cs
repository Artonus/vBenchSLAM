using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using vBenchSLAM.Addins;
using vBenchSLAM.Addins.Events;
using vBenchSLAM.Core.SystemMonitor;

namespace vBenchSLAM.Core.DockerCore
{
    public class DockerManager : IDockerManager
    {
        private readonly List<ProcessMonitor> _processMonitors;
        private readonly IDockerClient _client;
        private readonly ProcessRunner _runner;

        public DockerManager()
        {
            var uri = GetWslUri();
            _client = Settings.IsWsl
                ? new DockerClientConfiguration(uri).CreateClient()
                : new DockerClientConfiguration().CreateClient();

            _runner = new ProcessRunner();
            _runner.ProcessRegistered += RunnerProcessRegistered;

            _processMonitors = new List<ProcessMonitor>();
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

            var cont = containers.SingleOrDefault(c => c.Image == containerName);

            return cont;
        }

        private async Task<ContainerListResponse> GetContainerByIdAsync(string containerId)
        {
            var containers = await ListContainersAsync();

            var cont = containers.SingleOrDefault(c => c.ID == containerId);

            return cont;
        }

        public async Task<ContainerListResponse> DownloadAndBuildContainer(string repository, string containerName)
        {
            var containerInfo = $"{repository}:{containerName}";

            var pullExitCode = await _runner.PullContainer(containerInfo);

            // if (exitCode != 1)
            //     throw new FailedToPullImageException("Unable to pull image", exitCode, containerInfo);

            var buildExitCode = await _runner.BuildImage(containerInfo);
            // if (buildExitCode != 1)
            //     throw new FailedToBuildImageException("Unable to build image", exitCode, containerInfo);

            var image = await GetContainerByNameAsync(containerInfo);

            return image;
        }

        private void RunnerProcessRegistered(object sender, ProcessRegisteredEventArgs e)
        {
            if (e.Process is not VBenchProcess)
                return;

            var monitor = new ProcessMonitor((VBenchProcess) e.Process, RemoveProcessFromRegistryAction);

            _processMonitors.Add(monitor);
        }

        /// <summary>
        /// Function responsible for removing the references for the monitor after the process has exited
        /// </summary>
        /// <param name="processMonitor"></param>
        private void RemoveProcessFromRegistryAction(ProcessMonitor processMonitor)
        {
            if (_processMonitors.Contains(processMonitor))
            {
                _processMonitors.Remove(processMonitor);
                processMonitor.Dispose();
            }
        }
    }
}