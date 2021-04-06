using System;
using System.IO;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Serilog;
using vBenchSLAM.Addins;
using vBenchSLAM.Addins.ExtensionMethods;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.Core.SystemMonitor
{
    public class SystemResource : IProgress<ContainerStatsResponse>
    {
        private readonly ILogger _logger;
        private readonly string _tmpFilePath;

        public SystemResource(string outputFileName, ILogger logger)
        {
            _logger = logger;
            _tmpFilePath = Path.Combine(DirectoryHelper.GetMonitorsPath(), outputFileName);
        }

        public async void Report(ContainerStatsResponse value)
        {
            if (value is not null)
            {
                await SaveUsageToFileAsync(value);    
            }
        }

        private async Task SaveUsageToFileAsync(ContainerStatsResponse stats)
        {
            try
            {
                var fInfo = PrepareFile();

                await using (StreamWriter writer = fInfo.Exists
                    ? File.AppendText(fInfo.FullName)
                    : File.CreateText(fInfo.FullName))
                {
                    try
                    {
                        var usage = CalculateResourceUsage(stats);
                        // possible division by 0 exception while saving data
                        await writer.WriteLineAsync(usage.ParseAsCsvLiteral());
                    }
                    catch (InvalidOperationException ex)
                    {
                        _logger.Error(ex, "Could not calculate the performance for the current iteration");
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Could not access the file: {_tmpFilePath}, error: {ex}");
            }
        }

        private ResourceUsage CalculateResourceUsage(ContainerStatsResponse stats)
        {
            ulong ramUsage = 0, availableMem =0; 
            decimal ramPercentUsage = 0;
            if (stats.MemoryStats.Stats is not null)
            {
                ramUsage = stats.MemoryStats.Usage - stats.MemoryStats.Stats["cache"];
                availableMem = stats.MemoryStats.Limit;
                ramPercentUsage = ramUsage / availableMem * 100;    
            }  
            //TODO: cpu % usage returns sth over 300%
            ulong cpuDelta = stats.CPUStats.CPUUsage.TotalUsage - stats.PreCPUStats.CPUUsage.TotalUsage;
            ulong sysCpuDelta = stats.CPUStats.SystemUsage - stats.PreCPUStats.SystemUsage; 
            ulong onlineCPUs = stats.CPUStats.OnlineCPUs; 
            decimal cpuUsage = cpuDelta / sysCpuDelta * onlineCPUs * 100;
            return new ResourceUsage(cpuUsage, Convert.ToInt32(onlineCPUs), ramUsage, availableMem, ramPercentUsage);
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