using System;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.DesktopUI.Models
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
        public int Ram { get; set; }
        public decimal AvgCpuUsage { get; set; }
        public decimal AvgRamUsage { get; set; }

        public ResourceUsage[] ResourceUsages { get; set; }
    }
}