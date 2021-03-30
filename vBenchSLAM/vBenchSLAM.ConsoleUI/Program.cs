using System;
using vBenchSLAM.Core;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.Enums;
using vBenchSLAM.Core.MapParser;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Settings.Initialize();
                //var param = new RunnerParameters(MapperTypeEnum.OpenVslam, string.Empty, string.Empty);
                
                //var runner = new Runner(param);
                
                //runner.Run();

                var parser = new MapParser();
                parser.LoadObjectFromParsedMap(@"C:\Works\vBenchSLAM\Samples\map.msg",
                    @"C:\Works\vBenchSLAM\Samples\map_object.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
