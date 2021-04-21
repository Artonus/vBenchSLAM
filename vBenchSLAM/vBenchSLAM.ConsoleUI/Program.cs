using System;
using System.Diagnostics;
using Serilog;
using vBenchSLAM.Addins;
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

                // var parser = new OpenVslamParser();
                // parser.GetMapDataFromMessagePack(@"C:\Works\vBenchSLAM\Samples\map.msg");

                // Console.WriteLine(SizeHelper.SizeValue(16759115776L, Size.GB));
                // Console.WriteLine(SizeHelper.GetSizeSuffix(16759115776L, 2));

                var proc = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = "/bin/bash",
                        Arguments = "-c \"nvidia-smi --query-gpu=utilization.gpu --format=csv,noheader,nounits\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                while (proc.StandardOutput.EndOfStream == false)
                {
                    Console.WriteLine(proc.StandardOutput.ReadLine());
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "General exception");
            }
        }
    }
}
