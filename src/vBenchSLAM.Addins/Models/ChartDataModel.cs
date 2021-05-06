using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.Addins.Models
{
    public class ChartDataModel
    {
        /// <summary>
        /// Timestamp when the algorithm started running
        /// </summary>
        public DateTime Started { get; set; }
        /// <summary>
        /// Timestamp when the algorithm finished running
        /// </summary>
        public DateTime Finished { get; set; }
        /// <summary>
        /// The number of detected keyframes
        /// </summary>
        public int Keyframes { get; set; }
        /// <summary>
        /// The number of detected key points
        /// </summary>
        public int Keypoints { get; set; }
        /// <summary>
        /// The number of detected landmarks
        /// </summary>
        public int Landmarks { get; set; }
        /// <summary>
        /// Name of the framework used
        /// </summary>
        public string Framework { get; set; }
        /// <summary>
        /// Name of the used CPU
        /// </summary>
        public string Cpu { get; set; }
        /// <summary>
        /// The number of cores in a CPU
        /// </summary>
        public int Cores { get; set; }
        /// <summary>
        /// The amount of RAM available in bytes
        /// </summary>
        public ulong Ram { get; set; }
        /// <summary>
        /// Display value of the available RAM
        /// </summary>
        public string RamDisplay => SizeHelper.GetSizeSuffix(Ram, 2);
        /// <summary>
        /// Average usage of the CPU during the test of the algorithm
        /// </summary>
        public decimal AvgCpuUsage { get; set; }
        /// <summary>
        /// Display value of average usage of the CPU during the test of the algorithm
        /// </summary>
        public string AvgCpuUsageDisplay => DisplayAsPercentage(AvgCpuUsage);
        /// <summary>
        /// Average usage of the RAM during the test of the algorithm
        /// </summary>
        public decimal AvgRamUsage { get; set; }
        /// <summary>
        /// Display value of average usage of the RAM during the test of the algorithm
        /// </summary>
        public string AvgRamUsageDisplay => DisplayAsPercentage(AvgRamUsage);
        /// <summary>
        /// Average usage of the GPU during the test of the algorithm
        /// </summary>
        public decimal AvgGpuUsage { get; set; }
        /// <summary>
        /// Display value of average usage of the GPU during the test of the algorithm
        /// </summary>
        public string AvgGpuUsageDisplay => DisplayAsPercentage(AvgGpuUsage);
        /// <summary>
        /// List of the specific resource usages recorded
        /// </summary>
        public List<ResourceUsage> ResourceUsages { get; set; } = new();
        /// <summary>
        /// Adds the % sign at the end of a value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string DisplayAsPercentage(decimal value)
        {
            return $"{value}%";
        }
        /// <summary>
        /// Reads the <see cref="ChartDataModel"/> data from file
        /// </summary>
        /// <param name="runDataFilePath">path to the file</param>
        /// <returns><see cref="ChartDataModel"/> filled with the data from a file</returns>
        public ChartDataModel ParseRunDataFile(string runDataFilePath)
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
        /// <summary>
        /// Reads the <see cref="ResourceUsages"/> data from file
        /// </summary>
        /// <param name="resourceUsageFile">Path to the file that contains the record of a resource usage</param>
        /// <returns><see cref="ChartDataModel"/> filled with the data from a file</returns>
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
        /// <summary>
        /// Reads the hardware specifics of the current machine
        /// </summary>
        /// <returns></returns>
        public ChartDataModel ReadHardwareSpecifics()
        {
            Cores = Environment.ProcessorCount;
            if (ResourceUsages.Count > 0)
                Ram = ResourceUsages.First().MaxRamAvailable;

            // TODO: get cpu name
            Cpu = string.Empty;
            return this;
        }
        /// <summary>
        /// Calculates the average usage of the RAM, CPU and GPU based on a data in a model
        /// </summary>
        /// <returns><see cref="ChartDataModel"/> with calculated average values</returns>
        public ChartDataModel CalculateUsageAverages()
        {
            AvgCpuUsage = Math.Round(ResourceUsages.Average(r => r.ProcUsage), 2);
            AvgRamUsage = Math.Round(ResourceUsages.Average(r => r.RamPercentUsage), 2);
            AvgGpuUsage = Math.Round(ResourceUsages.Average(r => r.GPUUsage), 2);
            return this;
        }
    }
}