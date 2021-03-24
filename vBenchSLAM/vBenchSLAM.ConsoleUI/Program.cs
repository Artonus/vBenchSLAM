using System;
using vBenchSLAM.Core;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.Enums;

namespace vBenchSLAM.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Settings.Initialize();
                
                var runner = new Runner(MapperTypeEnum.OpenVslam);
                
                runner.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
