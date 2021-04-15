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
    public class SystemResourceMonitor : IProgress<ContainerStatsResponse>
    {
        private readonly ILogger _logger;
        private readonly string _tmpFilePath;

        public SystemResourceMonitor(string outputFileName, ILogger logger)
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
                        if (usage is not null)
                        {
                            await writer.WriteLineAsync(usage.ParseAsCsvLiteral());
                            _logger.Debug("Logged resource usage");
                        }
                    }
                    catch (Exception ex)
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
            
            decimal ramUsage = 0, availableMem =0; 
            decimal ramPercentUsage = 0;
            if (stats.MemoryStats.Stats is not null)
            {
                ramUsage = stats.MemoryStats.Usage - stats.MemoryStats.Stats["cache"];
                availableMem = stats.MemoryStats.Limit;
                ramPercentUsage = ramUsage / availableMem * 100;    
            }

            if (stats.MemoryStats.Limit == default)
            {
                // if didn't receive any data, then do not create the record
                return default;
            }
            //TODO: cpu % usage returns sth over 300%
            decimal cpuUsage = 0; 
            decimal cpuDelta = stats.CPUStats.CPUUsage.TotalUsage - stats.PreCPUStats.CPUUsage.TotalUsage;
            decimal sysCpuDelta = stats.CPUStats.SystemUsage - stats.PreCPUStats.SystemUsage; 
            decimal onlineCPUs = stats.CPUStats.OnlineCPUs;
            if (onlineCPUs == default)
            {
                onlineCPUs = stats.CPUStats.CPUUsage.PercpuUsage.Count;
            }

            if (sysCpuDelta > 0 && cpuDelta > 0)
            {
                cpuUsage = cpuDelta / sysCpuDelta * onlineCPUs * 100;    
            }
            
            return new ResourceUsage(cpuUsage, Convert.ToInt32(onlineCPUs), (ulong)ramUsage, (ulong)availableMem, ramPercentUsage);
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