using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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

        public async Task<bool> SendCommandAsync(string containerId, string command)
        {
            var container = await GetContainerByIdAsync(containerId);
            container.Command = command;

            var processRunner = new ProcessRunner();
            await processRunner.SendCommandToContainerAsync(containerId, command);

            //var containerParams = new ContainerExecCreateParameters()
            //{
            //    AttachStdout = true,
            //    AttachStderr = true,
            //    AttachStdin = false,
            //    Cmd = new List<string> { "bash -c " + command },
            //    //Privileged = false,
            //    //User = "root",
            //    //WorkingDir = "/openvslam/build"
            //};
            //Console.WriteLine(containerParams.Cmd[0]);
            //var response = await _client.Exec.ExecCreateContainerAsync(container.ID, containerParams);
            //if (string.IsNullOrEmpty(response.ID))
            //{
            //    return false;
            //}
            //ContainerExecInspectResponse inspection = await _client.Exec.InspectContainerExecAsync(response.ID);
            //var containerExecStartParams = new ContainerExecStartParameters
            //{
            //    AttachStdout = containerParams.AttachStdout,
            //    AttachStderr = containerParams.AttachStderr,
            //    AttachStdin = containerParams.AttachStdin,
            //    Cmd = containerParams.Cmd,
            //    Detach = false,
            //    Tty = true
            //};
            //var multiplexedStream = await _client.Exec.StartWithConfigContainerExecAsync(response.ID, containerExecStartParams);

            //inspection = await _client.Exec.InspectContainerExecAsync(response.ID);
            //await using (var fileStream = new FileStream("C:\\Works\\vBenchSLAM\\Samples\\output.txt", FileMode.Create))
            //{
            //    await multiplexedStream.CopyOutputToAsync(fileStream, fileStream, fileStream, new CancellationToken());
            //}
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
    }
}
