using Docker.DotNet.Models;

namespace vBenchSLAM.Addins.ExtensionMethods
{
    public static class ContainerStatsResponseExtensionMethods
    {
        public static string ParseAsCsvLiteral(this ContainerStatsResponse res)
        {
            //TODO: move as separate object
            ulong usedMem = 0, memoryUsage = 0;
            if (res.MemoryStats.Stats is not null)
            {
                usedMem = res.MemoryStats.Usage - res.MemoryStats.Stats["cache"];
                var availableMem = res.MemoryStats.Limit;
                memoryUsage = usedMem / availableMem * 100;    
            }   
            //TODO: fix calculation error resulting in 0
            var cpuDelta = res.CPUStats.CPUUsage.TotalUsage - res.PreCPUStats.CPUUsage.TotalUsage;
            var sysCpuDelta = res.CPUStats.SystemUsage - res.PreCPUStats.SystemUsage;
            var numberCpus = res.CPUStats.OnlineCPUs;
            var cpuUsage = cpuDelta / sysCpuDelta * numberCpus * 100;
            return $"{usedMem};{memoryUsage};{cpuUsage};";
        }
    }
}