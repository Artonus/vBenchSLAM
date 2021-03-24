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
    public class DockerManager : IDockerManager
    {
        protected readonly List<ProcessMonitor> ProcessMonitors;
        public IDockerClient Client { get; }
        protected readonly IProcessRunner Runner;

        public DockerManager(IProcessRunner runner)
        {
            
            var uri = GetWslUri();
            Client = Settings.IsWsl
                ? new DockerClientConfiguration(uri).CreateClient()
                : new DockerClientConfiguration().CreateClient();
            Runner = runner;
            Runner.ProcessRegistered += RunnerProcessRegistered;

            ProcessMonitors = new List<ProcessMonitor>();
        }

        private static Uri GetWslUri()
        {
            return new Uri($"tcp://127.0.0.1:{Settings.DockerWslPort}");
        }

        public virtual async Task<IList<ContainerListResponse>> ListContainersAsync()
        {
            IList<ContainerListResponse> containers = await Client.Containers.ListContainersAsync(
                new ContainersListParameters()
                {
                    All = true
                });
            return containers;
        }

        public virtual async Task<bool> StartContainerAsync(string container, string cmdArgs = "")
        {
            var parameters = new ContainerStartParameters()
            {
                DetachKeys = cmdArgs
            };
            var success = await Client.Containers.StartContainerAsync(container, parameters);

            return success;
        }

        public async Task<bool> StartContainerViaCommandLineAsync(string containerName, string startParameters = "", string containerCommand = "")
        {
            await Runner.StartContainerViaCommandLineAsync(containerName, startParameters, containerCommand);
            //Client.Containers.GetContainerStatsAsync()
            //TODO: return real value
            return true;
        }

        public virtual async Task<bool> StopContainerAsync(string containerId)
        {
            var parameters = new ContainerStopParameters()
            {
                WaitBeforeKillSeconds = 5
            };
            var success = await Client.Containers.StopContainerAsync(containerId, parameters);
            return success;
        }

        public virtual async Task<bool> SendCommandAsync(string containerId, string command)
        {
            var container = await GetContainerByIdAsync(containerId);
            container.Command = command;

            await Runner.SendCommandToContainerAsync(containerId, command);

            return true;
        }

        public virtual async Task<ContainerListResponse> GetContainerByNameAsync(string containerName)
        {
            var containers = await ListContainersAsync();

            var cont = containers.SingleOrDefault(c => c.Image == containerName);

            return cont;
        }

        public virtual async Task<ContainerListResponse> GetContainerByIdAsync(string containerId)
        {
            var containers = await ListContainersAsync();

            var cont = containers.SingleOrDefault(c => c.ID == containerId);

            return cont;
        }

        public virtual async Task<ContainerListResponse> DownloadAndBuildContainer(string repository,
            string containerName)
        {
            var containerInfo = $"{repository}:{containerName}";

            var pullExitCode = await Runner.PullContainer(containerInfo);

            var buildExitCode = await Runner.BuildImage(containerInfo);

            var image = await GetContainerByNameAsync(containerInfo);

            return image;
        }

        protected virtual void RunnerProcessRegistered(object sender, ProcessRegisteredEventArgs e)
        {
            if (e.Process is not VBenchProcess)
                return;

            var monitor = new ProcessMonitor((VBenchProcess) e.Process, RemoveProcessFromRegistryAction);

            ProcessMonitors.Add(monitor);
        }

        /// <summary>
        /// Function responsible for removing the references for the monitor after the process has exited
        /// </summary>
        /// <param name="processMonitor"></param>
        protected virtual void RemoveProcessFromRegistryAction(ProcessMonitor processMonitor)
        {
            if (ProcessMonitors.Contains(processMonitor))
            {
                ProcessMonitors.Remove(processMonitor);
                processMonitor.Dispose();
            }
        }
    }
}