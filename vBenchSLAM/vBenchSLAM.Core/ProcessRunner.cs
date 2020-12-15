using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace vBenchSLAM.Core
{
    public class ProcessRunner
    {
        public ProcessRunner()
        {

        }

        public async Task<int> SendCommandToContainerAsync(string containerId, string command)
        {
            var baseProgram = Settings.IsUnix ? "bash" : "cmd.exe";
            var execCmdOption = Settings.IsUnix ? "-c" : "/C";
            var prefix = Settings.IsWsl ? "wsl" : string.Empty;

            var args = $@"{execCmdOption} ""{prefix} docker exec -it {containerId} bash -c {command}""";
            return await RunProcessAsync(baseProgram, args);
        }

        public static async Task<int> RunProcessAsync(string fileName, string args)
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
            using (var process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            })
            {
                return await RunProcessAsync(process).ConfigureAwait(false);
            }
                

        }
        private static Task<int> RunProcessAsync(Process process)
        {
            var tcs = new TaskCompletionSource<int>();

            process.Exited += (s, ea) =>
            {
                tcs.SetResult(process.ExitCode);
                Console.WriteLine($"Process has exited with code: {process.ExitCode}");
            };
            process.OutputDataReceived += (s, ea) => Console.WriteLine(ea.Data);
            process.ErrorDataReceived += (s, ea) => Console.WriteLine("ERR: " + ea.Data);

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
    }
}
