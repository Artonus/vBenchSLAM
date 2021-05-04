using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.Addins.Models
{
    public class ChartDataModel
    {
        public DateTime Started { get; set; }
        public DateTime Finished { get; set; }
        public int Keyframes { get; set; }
        public int Keypoints { get; set; }
        public int Landmarks { get; set; }
        public string Framework { get; set; }
        public string Cpu { get; set; }
        public int Cores { get; set; }
        public ulong Ram { get; set; }
        
        public string RamDisplay => SizeHelper.GetSizeSuffix(Ram, 2);

        public decimal AvgCpuUsage { get; set; }
        public string AvgCpuUsageDisplay => DisplayAsPercentage(AvgCpuUsage);
        public decimal AvgRamUsage { get; set; }
        public string AvgRamUsageDisplay => DisplayAsPercentage(AvgRamUsage);
        public decimal AvgGpuUsage { get; set; }
        public string AvgGpuUsageDisplay => DisplayAsPercentage(AvgGpuUsage);

        public List<ResourceUsage> ResourceUsages { get; set; } = new();
        
        private string DisplayAsPercentage(decimal value)
        {
            return $"{value}%";
        }
        public  ChartDataModel ParseRunDataFile(string runDataFilePath)
        {
            var fileContent = File.ReadAllLines(runDataFilePath);

            for (int i = 0; i < fileContent.Length; i++) // we skip the first line as it is the header line
            {
                if (i == 0 || i == 2) // skip the header lines
                    continue;
                var split = fileContent[i].Split(';', StringSplitOptions.RemoveEmptyEntries);
                
                if (i == 1)
                {
                    Started = DateTime.Parse(split[0]);
                    Finished = DateTime.Parse(split[1]);
                    Framework = split[2];
                }
                if (i == 3)
                {
                    var data = MapData.FromCsvLiteral(fileContent[i]);
                    Keyframes = data.Keyframes;
                    Keypoints = data.Keypoints;
                    Landmarks = data.Landmarks;
                }
            }
            return this;
        }
        public ChartDataModel ParseResourceUsage(string resourceUsageFile)
        {
            var lines = File.ReadAllLines(resourceUsageFile);

            foreach (var line in lines)
            {
                var parsedModel = ResourceUsage.FromCsvLiteral(line);
                ResourceUsages.Add(parsedModel);
            }

            return this;
        }

        public ChartDataModel ReadHardwareSpecifics()
        {
            Cores = Environment.ProcessorCount;
            if (ResourceUsages.Count > 0)
                Ram = ResourceUsages.First().MaxRamAvailable;

            //TODO: get cpu name
            Cpu = string.Empty;
            return this;
        }

        public ChartDataModel CalculateUsageAverages()
        {
            AvgCpuUsage = Math.Round(ResourceUsages.Average(r => r.ProcUsage), 2);
            AvgRamUsage = Math.Round(ResourceUsages.Average(r => r.RamPercentUsage), 2);
            AvgGpuUsage = Math.Round(ResourceUsages.Average(r => r.GPUUsage), 2);
            return this;
        }
    }
}