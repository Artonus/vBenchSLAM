using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using vBenchSLAM.Addins;
using vBenchSLAM.Addins.Events;

namespace vBenchSLAM.Core.ProcessRunner
{
    internal class ProcessRunner : IProcessRunner
    {
        /// <summary>
        /// Event handler of a new process being registered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ProcessRegisteredEventHandler(object sender, ProcessRegisteredEventArgs e);
        /// <summary>
        /// Event that occurs when a new process is registered
        /// </summary>
        public event ProcessRegisteredEventHandler ProcessRegistered;
        /// <summary>
        /// Base program to run on a system
        /// </summary>
        protected string BaseProgram { get; }
        /// <summary>
        /// option to the base program to execute the command
        /// </summary>
        protected string ExecCmdOption { get; }

        public ProcessRunner()
        {
            BaseProgram = Settings.IsUnix ? "/bin/bash" : "cmd.exe";
            ExecCmdOption = Settings.IsUnix ? "-c" : "/C";
        }

        /// <summary>
        /// <inheritdoc cref="IProcessRunner.StartContainerViaCommandLineAsync"/>
        /// </summary>
        [Obsolete]
        public virtual async Task<int> StartContainerViaCommandLineAsync(string containerName, string startParameters,
            string containerCommand = "")
        {
            var args =
                $"{ExecCmdOption} \"{GetDockerRunCommand(containerName, startParameters, containerCommand)}\"";
            return await RunProcessAsync(BaseProgram, args, false);
        }
        /// <summary>
        /// <inheritdoc cref="IProcessRunner.SendCommandToContainerAsync"/>
        /// </summary>
        /// <param name="containerId"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual async Task<int> SendCommandToContainerAsync(string containerId, string command)
        {
            var args = $@"{ExecCmdOption} ""docker exec -it {containerId} bash -c {command}""";
            return await RunProcessAsync(BaseProgram, args);
        }
        /// <summary>
        /// <inheritdoc cref="IProcessRunner.PullContainerAsync"/>
        /// </summary>
        /// <param name="containerInfo"></param>
        /// <returns></returns>
        public virtual async Task<int> PullContainerAsync(string containerInfo)
        {
            var args = $@"{ExecCmdOption} ""docker pull {containerInfo}""";
            return await RunProcessAsync(BaseProgram, args, false);
        }
        /// <summary>
        /// <inheritdoc cref="IProcessRunner.BuildImageAsync"/>
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public virtual async Task<int> BuildImageAsync(string containerName)
        {
            var args = $@"{ExecCmdOption} ""docker container create {containerName}""";
            return await RunProcessAsync(BaseProgram, args, false);
        }
        /// <summary>
        /// <inheritdoc cref="IProcessRunner.RunProcessAsync"/>
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual async Task<int> RunProcessAsync(string fileName, string args)
        {
            return await RunProcessAsync(fileName, args, true);
        }
        /// <summary>
        /// <inheritdoc cref="IProcessRunner.RunProcessAsync"/>
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="args"></param>
        /// <param name="riseCustomEvents"></param>
        /// <returns></returns>
        protected async Task<int> RunProcessAsync(string fileName, string args, bool riseCustomEvents)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            Console.WriteLine($"{fileName} {args}");
            using (var process = new Process())
            {
                process.StartInfo = startInfo;
                if (riseCustomEvents)
                    OnProcessRegistered(new ProcessRegisteredEventArgs(process));

                return await RunProcessAsync(process).ConfigureAwait(false);
            }
        }
        /// <summary>
        /// Asynchronously runs the process
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private static Task<int> RunProcessAsync(Process process)
        {
            var tcs = new TaskCompletionSource<int>();
            void OnProcessExited(object sender, EventArgs ea)
            {
                int exitCode = 1;
                try
                {
                    exitCode = process.ExitCode;
                    Console.WriteLine($"Process has exited with code: {process.ExitCode}");
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    tcs.SetResult(exitCode);
                }
            }
            
            process.Exited += OnProcessExited;
            process.OutputDataReceived += ProcessOnOutputDataReceived;
            process.ErrorDataReceived += ProcessOnErrorDataReceived;

            bool started = process.Start();
            Thread.Sleep(200);
            if (!started)
            {
                //you may allow for the process to be re-used (started = false) 
                //but I'm not sure about the guarantees of the Exited event in such a case
                throw new InvalidOperationException("Could not start process: " + process);
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            //await process.WaitForExitAsync();

            return tcs.Task;
        }
        /// <summary>
        /// Outputs the data received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }
        /// <summary>
        /// Outputs the error data received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data) == false)
            {
                Console.WriteLine("ERR: " + e.Data);
            }
        }
        /// <summary>
        /// Invokes the <see cref="ProcessRegistered"/> event
        /// </summary>
        /// <param name="e"></param>
        private void OnProcessRegistered(ProcessRegisteredEventArgs e)
        {
            ProcessRegistered?.Invoke(this, e);
        }
        /// <summary>
        /// Returns the command to start the container
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="startParameters"></param>
        /// <param name="containerCommand"></param>
        /// <returns></returns>
        protected string GetDockerRunCommand(string containerName, string startParameters, string containerCommand = "")
        {
            var cmd = $"docker run {startParameters} {containerName}";
            if (string.IsNullOrEmpty(containerCommand) == false)
            {
                cmd += $" \"{containerCommand}\"";
            }

            return cmd;
        }
        /// <summary>
        /// <inheritdoc cref="IProcessRunner.CaptureCommandOutput"/>
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string CaptureCommandOutput(string command)
        {
            var proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = BaseProgram,
                    Arguments = $"{ExecCmdOption} \"{command}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            string output = string.Empty;
            
            while (proc.StandardOutput.EndOfStream == false)
            {
                output += proc.StandardOutput.ReadLine();
            }

            return output;
        }
        /// <summary>
        /// <inheritdoc cref="IProcessRunner.EnablePangolinViewerAsync"/>
        /// </summary>
        /// <returns></returns>
        public async Task EnablePangolinViewerAsync()
        {
            await RunProcessAsync(BaseProgram, $"{ExecCmdOption} \"xhost + \"", false);
        }
    }
}