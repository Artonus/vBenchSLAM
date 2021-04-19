using System;
using Serilog;
using vBenchSLAM.Core;
using vBenchSLAM.Core.MapParser;

namespace vBenchSLAM.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Settings.Initialize();
                //var param = new RunnerParameters(MapperType.OpenVslam, string.Empty, string.Empty);
                
                //var runner = new Runner(param);
                
                //runner.Run();

                var parser = new OpenVslamParser();
                parser.GetMapDataFromMessagePack(@"C:\Works\vBenchSLAM\Samples\map.msg");
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "General exception");
            }
        }
    }
}
