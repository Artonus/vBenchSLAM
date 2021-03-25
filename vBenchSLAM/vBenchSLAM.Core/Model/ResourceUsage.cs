using System;

namespace vBenchSLAM.Core.Model
{
    public class ResourceUsage
    {
        /// <summary>
        /// RAM usage in bytes
        /// </summary>
        public ulong RamUsage { get; }
        /// <summary>
        /// Total RAM available in bytes
        /// </summary>
        public ulong MaxRamAvailable { get; set; }
        /// <summary>
        /// RAM usage in %
        /// </summary>
        public decimal RamPercentUsage { get; set; }
        /// <summary>
        /// Number of active processor logical units
        /// </summary>
        public int OnlineCPUs { get; set; }
        /// <summary>
        /// CPU usage as % of all CPU usage
        /// </summary>
        public decimal ProcUsage { get; }
        
        public ResourceUsage(decimal procUsage, int onlineCPUs, ulong ramUsage, ulong maxRamAvailable, decimal ramPercentUsage)
        {
            ProcUsage = procUsage;
            RamUsage = ramUsage;
            MaxRamAvailable = maxRamAvailable;
            RamPercentUsage = ramPercentUsage;
            OnlineCPUs = onlineCPUs;
        }

        public string ParseAsCsvLiteral()
        {
            return $"{RamUsage};{MaxRamAvailable};{Math.Round(RamPercentUsage, 2)};{OnlineCPUs};{Math.Round(ProcUsage, 2)}";
        }

        public static string AsCsvLiteral(ResourceUsage model)
        {
            return model.ParseAsCsvLiteral();
        }
        public static ResourceUsage FromCsvLiteral(string line)
        {
            throw new NotImplementedException();
        }
    }
}