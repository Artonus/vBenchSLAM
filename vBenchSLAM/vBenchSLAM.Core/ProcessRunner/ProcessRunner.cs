using System;
using System.Diagnostics;
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
        protected string Prefix { get; }

        public ProcessRunner()
        {
            BaseProgram = Settings.IsUnix ? "/bin/bash" : "cmd.exe";
            ExecCmdOption = Settings.IsUnix ? "-c" : "/C";
            Prefix = Settings.IsWsl ? "wsl " : string.Empty;
        }

        /// <summary>
        /// <inheritdoc cref="IProcessRunner.StartContainerViaCommandLineAsync"/>
        /// </summary>
        public virtual async Task<int> StartContainerViaCommandLineAsync(string containerName, string startParameters, string containerCommand = "")
        {
            var args = $"{ExecCmdOption} \"{Prefix} {GetDockerRunCommand(containerName, startParameters, containerCommand)}\"";
            return await RunProcessAsync(BaseProgram, args, false);
        }

        public virtual async Task<int> SendCommandToContainerAsync(string containerId, string command)
        {
            var args = $@"{ExecCmdOption} ""{Prefix} docker exec -it {containerId} bash -c {command}""";
            return await RunProcessAsync(BaseProgram, args);
        }

        public virtual async Task<int> PullContainer(string containerInfo)
        {
            var args = $@"{ExecCmdOption} ""{Prefix} docker pull {containerInfo}""";
            return await RunProcessAsync(BaseProgram, args, false);
        }

        public virtual async Task<int> BuildImage(string containerName)
        {
            var args = $@"{ExecCmdOption} ""{Prefix} docker container create {containerName}""";
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
            using (var process = new VBenchProcess(startInfo, riseCustomEvents))
            {
                // if (riseCustomEvents)
                //     OnProcessRegistered(new ProcessRegisteredEventArgs(process));

                return await RunProcessAsync(process).ConfigureAwait(true);
            }
        }

        protected static Task<int> RunProcessAsync(VBenchProcess process)
        {
            var tcs = new TaskCompletionSource<int>();
            process.Exited += (s, ea) =>
            {
                Console.WriteLine($"Process has exited with code: {process.ExitCode}");
                tcs.SetResult(process.ExitCode);
            };
            process.OutputDataReceived += ProcessOnOutputDataReceived;
            process.ErrorDataReceived += ProcessOnErrorDataReceived;

            bool started = process.Start();
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
            return $"docker run {startParameters} {containerName} {containerCommand}";
        }
    }
}