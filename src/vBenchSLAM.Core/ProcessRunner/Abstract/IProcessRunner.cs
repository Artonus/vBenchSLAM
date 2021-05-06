using System;
using System.Threading.Tasks;

namespace vBenchSLAM.Core.ProcessRunner
{
    internal interface IProcessRunner
    {
        /// <summary>
        /// Asynchronously starts the container using the command line
        /// </summary>
        /// <param name="containerName">The name of the container image including tag</param>
        /// <param name="startParameters">Map parameters of the container</param>
        /// <param name="containerCommand">Additional command to be executed by the container</param>
        /// <returns></returns>
        [Obsolete]
        public Task<int> StartContainerViaCommandLineAsync(string containerName, string startParameters, string containerCommand = "");
        /// <summary>
        /// Asynchronously sends the command to the container
        /// </summary>
        /// <param name="containerId"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<int> SendCommandToContainerAsync(string containerId, string command);
        /// <summary>
        /// Asynchronously pulls the container from the repository
        /// </summary>
        /// <param name="containerInfo"></param>
        /// <returns></returns>
        Task<int> PullContainerAsync(string containerInfo);
        /// <summary>
        /// Asynchronously builds the container
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        Task<int> BuildImageAsync(string containerName);
        /// <summary>
        /// Asynchronously runs the process
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<int> RunProcessAsync(string fileName, string args);
        /// <summary>
        /// Asynchronously enables the Pangolin Viewer
        /// </summary>
        /// <returns></returns>
        Task EnablePangolinViewerAsync();
        /// <summary>
        /// Captures the output of the command 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string CaptureCommandOutput(string command);
    }
}