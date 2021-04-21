﻿using System;
using System.Collections.Generic;
using vBenchSLAM.Addins;
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
    }
}