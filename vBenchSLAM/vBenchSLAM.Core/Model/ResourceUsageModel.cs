using System;

namespace vBenchSLAM.Core.Model
{
    public class ResourceUsageModel
    {
        /// <summary>
        /// RAM usage in bytes
        /// </summary>
        public long RamUsage { get; }
        /// <summary>
        /// CPU usage as % of all CPU usage
        /// </summary>
        public decimal ProcUsage { get; }
        public ResourceUsageModel(decimal procUsage, long ramUsage)
        {
            ProcUsage = procUsage;
            RamUsage = ramUsage;
        }

        public string ParseAsCsvLiteral()
        {
            return $"{RamUsage};{Math.Round(ProcUsage, 2)}";
        }

        public static string AsCsvLiteral(ResourceUsageModel model)
        {
            return model.ParseAsCsvLiteral();
        }
        public static ResourceUsageModel FromCsvLiteral(string line)
        {
            throw new NotImplementedException();
        }
    }
}