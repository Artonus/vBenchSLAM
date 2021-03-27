using System;
using vBenchSLAM.Core;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.Enums;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Settings.Initialize();
                var param = new RunnerParameters(MapperTypeEnum.OpenVslam, string.Empty, string.Empty);
                
                var runner = new Runner(param);
                
                runner.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
