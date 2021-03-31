﻿using System;
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
                //var param = new RunnerParameters(MapperTypeEnum.OpenVslam, string.Empty, string.Empty);
                
                //var runner = new Runner(param);
                
                //runner.Run();

                var parser = new BaseParser();
                parser.LoadObjectFromParsedMap(@"C:\Works\vBenchSLAM\Samples\map.msg",
                    @"C:\Works\vBenchSLAM\Samples\map_object.txt");
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "General exception");
            }
        }
    }
}
