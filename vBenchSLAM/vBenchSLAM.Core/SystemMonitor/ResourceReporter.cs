using System;
using Docker.DotNet.Models;

namespace vBenchSLAM.Core.SystemMonitor
{
    public class ResourceReporter : IProgress<ContainerStatsResponse>
    {
        public void Report(ContainerStatsResponse value)
        {
            Console.WriteLine("Hi, I have reported :)");
        }
    }
}