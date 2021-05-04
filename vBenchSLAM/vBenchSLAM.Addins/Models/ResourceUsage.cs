using System;
using vBenchSLAM.Addins.Abstract;

namespace vBenchSLAM.Core.Model
{
    public class ResourceUsage : ICsvParsable
    {
        /// <summary>
        /// RAM usage in bytes
        /// </summary>
        public ulong RamUsage { get; }
        /// <summary>
        /// Total RAM available in bytes
        /// </summary>
        public ulong MaxRamAvailable { get; init; }
        /// <summary>
        /// RAM usage in %
        /// </summary>
        public decimal RamPercentUsage { get; init; }
        /// <summary>
        /// Number of active processor logical units
        /// </summary>
        public int OnlineCPUs { get; init; }
        /// <summary>
        /// CPU usage as % of all CPU usage
        /// </summary>
        public decimal ProcUsage { get; }
        /// <summary>
        /// GPU usage as % of all GPU usage
        /// </summary>
        public decimal GPUUsage { get; }
        
        public ResourceUsage(decimal procUsage, int onlineCPUs, ulong ramUsage, ulong maxRamAvailable, decimal ramPercentUsage, decimal gpuUsage)
        {
            ProcUsage = procUsage;
            RamUsage = ramUsage;
            MaxRamAvailable = maxRamAvailable;
            RamPercentUsage = ramPercentUsage;
            OnlineCPUs = onlineCPUs;
            GPUUsage = gpuUsage;
        }

        public string GetCsvHeaderRow()
        {
            return
                $"{nameof(RamUsage)};{nameof(MaxRamAvailable)};{nameof(RamPercentUsage)};{nameof(OnlineCPUs)};{nameof(ProcUsage)}";
        }

        public string ParseAsCsvLiteral()
        {
            return $"{RamUsage};{MaxRamAvailable};{Math.Round(RamPercentUsage, 2)};{OnlineCPUs};{Math.Round(ProcUsage, 2)};{Math.Round(GPUUsage, 2)}";
        }

        public static string AsCsvLiteral(ResourceUsage model)
        {
            return model.ParseAsCsvLiteral();
        }
        public static ResourceUsage FromCsvLiteral(string line)
        {
            string[] values = line.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            ulong ramUsage = ulong.Parse(values[0]);
            ulong maxRam = ulong.Parse(values[1]);
            decimal ramPercentUsage = decimal.Parse(values[2]);
            int onlineCpus = int.Parse(values[3]);
            decimal procUsage = decimal.Parse(values[4]);
            decimal gpuUsage = decimal.Parse(values[5]);
            return new ResourceUsage(procUsage, onlineCpus, ramUsage, maxRam, ramPercentUsage, gpuUsage);
        }
    }
}