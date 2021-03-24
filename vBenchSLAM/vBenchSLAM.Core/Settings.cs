using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace vBenchSLAM.Core
{
    /// <summary>
    /// Application Settings
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// The port the docker in WSL2 should be configured
        /// </summary>
        public const int DockerWslPort = 2375;

        /// <summary>
        /// The name of the repository containing docker images of vSLAM frameworks
        /// </summary>
        public const string VBenchSlamRepositoryName = "artonus/vbenchslam";

        public static bool IsWsl { get; private set; }

        public static bool IsUnix { get; private set; }

        public static void Initialize()
        {
            CheckWsl();
            IsUnix = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                     RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        private static bool CheckWsl()
        {
            var retVal = false;
            var system = Environment.OSVersion;
            if (system.Platform == PlatformID.Win32NT)
            {
                Process proc = new Process();
                var startInfo = new ProcessStartInfo("cmd.exe", "/C wsl -l")
                {
                    UseShellExecute = false, RedirectStandardOutput = true
                };
                proc.StartInfo = startInfo;
                if (proc.Start())
                {
                    var output = proc.StandardOutput.ReadToEnd().Trim('\0');
                    Console.WriteLine(output);
                    proc.WaitForExit();
                    if (string.IsNullOrEmpty(output) == false)
                        retVal = true;
                }
            }

            IsWsl = retVal;
            return retVal;
        }
    }
}