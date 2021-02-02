using System;
using System.Diagnostics;
using System.Threading.Tasks;
using vBenchSLAM.Addins;
using vBenchSLAM.Addins.EventArgs;

namespace vBenchSLAM.Core
{
    public class ProcessRunner
    {
        public delegate void ProcessRegisteredEventHandler(object sender, ProcessRegisteredEventArgs e);

        public event ProcessRegisteredEventHandler ProcessRegistered;
        
        private string BaseProgram { get; }
        private string ExecCmdOption { get; }
        private string Prefix { get; }
        public ProcessRunner()
        {
            BaseProgram = Settings.IsUnix ? "bash" : "cmd.exe";
            ExecCmdOption = Settings.IsUnix ? "-c" : "/C";
            Prefix = Settings.IsWsl ? "wsl" : string.Empty;
        }

        public async Task<int> SendCommandToContainerAsync(string containerId, string command)
        {
            var args = $@"{ExecCmdOption} ""{Prefix} docker exec -it {containerId} bash -c {command}""";
            return await RunProcessAsync(BaseProgram, args);
        }

        public async Task<int> PullContainer(string containerInfo)
        {
            var args = $@"{ExecCmdOption} ""{Prefix} docker pull {containerInfo}""";
            return await RunProcessAsync(BaseProgram, args);
        }
        
        public async Task<int> BuildImage(string containerName)
        {
            var args = $@"{ExecCmdOption} ""{Prefix} docker container create {containerName}""";
            return await RunProcessAsync(BaseProgram, args);
        }

        public async Task<int> RunProcessAsync(string fileName, string args)
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
            using (var process = new VBenchProcess
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            })
            {
                OnProcessRegistered(new ProcessRegisteredEventArgs(process));
                return await RunProcessAsync(process).ConfigureAwait(false);
            }
        }

        private static Task<int> RunProcessAsync(Process process)
        {
            var tcs = new TaskCompletionSource<int>();

            // process.Exited += (s, ea) =>
            // {
            //     tcs.SetResult(process.ExitCode);
            //     Console.WriteLine($"Process has exited with code: {process.ExitCode}");
            // };
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

            return tcs.Task;
        }

        #region Events

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
        #endregion

        private void OnProcessRegistered(ProcessRegisteredEventArgs e)
        {
            ProcessRegistered?.Invoke(this, e);
        }
    }
}