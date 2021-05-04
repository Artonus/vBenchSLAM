using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using vBenchSLAM.Addins;
using vBenchSLAM.Addins.Events;

namespace vBenchSLAM.Core.ProcessRunner
{
    public class ProcessRunner : IProcessRunner
    {
        public delegate void ProcessRegisteredEventHandler(object sender, ProcessRegisteredEventArgs e);

        public event ProcessRegisteredEventHandler ProcessRegistered;

        protected string BaseProgram { get; }
        protected string ExecCmdOption { get; }
        protected string CommandPrefix { get; }

        public ProcessRunner()
        {
            BaseProgram = Settings.IsUnix ? "/bin/bash" : "cmd.exe";
            ExecCmdOption = Settings.IsUnix ? "-c" : "/C";
            CommandPrefix = Settings.IsWsl ? "wsl " : string.Empty;
        }

        /// <summary>
        /// <inheritdoc cref="IProcessRunner.StartContainerViaCommandLineAsync"/>
        /// </summary>
        public virtual async Task<int> StartContainerViaCommandLineAsync(string containerName, string startParameters,
            string containerCommand = "")
        {
            var args =
                $"{ExecCmdOption} \"{CommandPrefix} {GetDockerRunCommand(containerName, startParameters, containerCommand)}\"";
            return await RunProcessAsync(BaseProgram, args, false);
        }

        public virtual async Task<int> SendCommandToContainerAsync(string containerId, string command)
        {
            var args = $@"{ExecCmdOption} ""{CommandPrefix} docker exec -it {containerId} bash -c {command}""";
            return await RunProcessAsync(BaseProgram, args);
        }

        public virtual async Task<int> PullContainer(string containerInfo)
        {
            var args = $@"{ExecCmdOption} ""{CommandPrefix} docker pull {containerInfo}""";
            return await RunProcessAsync(BaseProgram, args, false);
        }

        public virtual async Task<int> BuildImage(string containerName)
        {
            var args = $@"{ExecCmdOption} ""{CommandPrefix} docker container create {containerName}""";
            return await RunProcessAsync(BaseProgram, args, false);
        }

        public virtual async Task<int> RunProcessAsync(string fileName, string args)
        {
            return await RunProcessAsync(fileName, args, true);
        }

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
            using (var process = new VBenchProcess(startInfo, riseCustomEvents))
            {
                if (riseCustomEvents)
                    OnProcessRegistered(new ProcessRegisteredEventArgs(process));

                return await RunProcessAsync(process).ConfigureAwait(false);
            }
        }

        protected static Task<int> RunProcessAsync(VBenchProcess process)
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

        private static void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        private static void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data) == false)
            {
                Console.WriteLine("ERR: " + e.Data);
            }
        }

        private void OnProcessRegistered(ProcessRegisteredEventArgs e)
        {
            ProcessRegistered?.Invoke(this, e);
        }

        protected string GetDockerRunCommand(string containerName, string startParameters, string containerCommand = "")
        {
            var cmd = $"docker run {startParameters} {containerName}";
            if (string.IsNullOrEmpty(containerCommand) == false)
            {
                cmd += $" \"{containerCommand}\"";
            }

            return cmd;
        }

        public string CaptureCommandOutput(string command)
        {
            var proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = BaseProgram,
                    Arguments = $"{ExecCmdOption} \"{CommandPrefix} {command}\"",
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

        public async Task EnablePangolinViewer()
        {
            await RunProcessAsync(BaseProgram, $"{ExecCmdOption} \"xhost + \"", false);
        }
    }
}