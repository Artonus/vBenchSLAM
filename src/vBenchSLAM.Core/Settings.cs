using System.IO;
using System.Runtime.InteropServices;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using vBenchSLAM.Addins;

namespace vBenchSLAM.Core
{
    /// <summary>
    /// Application Settings
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// The name of the repository containing docker images of VSLAM frameworks
        /// </summary>
        public const string VBenchSlamRepositoryName = "artonus/vbenchslam";
        /// <summary>
        /// The name of the file that stores the log of all the successful runs
        /// </summary>
        public const string RunLogFileName = "runLog.txt";
        /// <summary>
        /// The name of the file that stores the run and framework specific information
        /// </summary>
        public const string RunDataFileName = "data.csv";

        public static bool IsUnix { get; private set; }

        public static void Initialize()
        {
            IsUnix = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                     RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

            ConfigureLogger();
        }
        /// <summary>
        /// Configures the logger instance to output to file and console
        /// </summary>
        private static void ConfigureLogger()
        {
            string logFilePath = Path.Combine(DirectoryHelper.GetAppDataFolder(), "logs/vBenchSLAM.log");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .WriteTo.File(logFilePath)
                .CreateLogger();
        }
    }
}