using System;
using System.IO;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using vBenchSLAM.Addins;
using vBenchSLAM.Addins.ExtensionMethods;

namespace vBenchSLAM.Core.SystemMonitor
{
    public class ResourceReporter : IProgress<ContainerStatsResponse>
    {
        private readonly string _tmpFilePath;

        public ResourceReporter()
        {
            var currTime = DateTime.Now;
            var tmpPath = DirectoryHelper.GetTempPath();
            _tmpFilePath = @$"{tmpPath}monitors/{currTime.FormatAsFileNameCode()}.csv";
        }

        public void Report(ContainerStatsResponse value)
        {
            if (value is not null)
            {
                SaveUsageToFileAsync(value);    
            }
            
        }

        private async Task SaveUsageToFileAsync(ContainerStatsResponse model)
        {
            try
            {
                var fInfo = PrepareFile();

                await using (StreamWriter writer = fInfo.Exists
                    ? File.AppendText(fInfo.FullName)
                    : File.CreateText(fInfo.FullName))
                {
                    await writer.WriteLineAsync(model.ParseAsCsvLiteral());
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Could not access the file: {_tmpFilePath}, error: {ex}");
            }
        }

        private FileInfo PrepareFile()
        {
            var fInfo = new FileInfo(_tmpFilePath);
            if (fInfo.Exists == false)
            {
                if (string.IsNullOrEmpty(fInfo.DirectoryName) == false
                    && Directory.Exists(fInfo.DirectoryName) == false)
                {
                    Directory.CreateDirectory(fInfo.DirectoryName);
                }
            }
            return fInfo;
        }
    }
}