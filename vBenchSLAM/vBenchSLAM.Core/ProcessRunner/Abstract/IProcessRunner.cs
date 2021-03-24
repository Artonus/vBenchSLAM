using System.Threading.Tasks;

namespace vBenchSLAM.Core.ProcessRunner
{
    public interface IProcessRunner
    {
        event ProcessRunner.ProcessRegisteredEventHandler ProcessRegistered;

        /// <summary>
        /// Asynchronously starts the container using the command line
        /// </summary>
        /// <param name="containerName">The name of the container image including tag</param>
        /// <param name="startParameters">Start parameters of the container</param>
        /// <param name="containerCommand">Additional command to be executed by the container</param>
        /// <returns></returns>
        public Task<int> StartContainerViaCommandLineAsync(string containerName, string startParameters, string containerCommand = "");

        Task<int> SendCommandToContainerAsync(string containerId, string command);
        Task<int> PullContainer(string containerInfo);
        Task<int> BuildImage(string containerName);
        Task<int> RunProcessAsync(string fileName, string args);
    }
}