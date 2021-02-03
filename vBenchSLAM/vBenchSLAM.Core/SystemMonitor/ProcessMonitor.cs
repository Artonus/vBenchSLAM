using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using vBenchSLAM.Addins;
using vBenchSLAM.Addins.Events;
using vBenchSLAM.Addins.ExtensionMethods;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.Core.SystemMonitor
{
    public class ProcessMonitor : IDisposable
    {
        private readonly Process _process;
        private readonly Timer _timer;
        private string _tmpFilePath;
        private PerformanceCounter _performanceCounter;

        public ProcessMonitor(VBenchProcess process)
        {
            _process = process;
            process.ProcessStarted += ProcessOnProcessStarted;

            _timer = new Timer
            {
                Interval = 1000
            };
            _timer.Elapsed += TimerOnElapsed;
        }

        private void RecordProcessUsage()
        {
            var procCpuUsage = (decimal) (_performanceCounter.NextValue() / Environment.ProcessorCount);
            var model = new ResourceUsageModel(procCpuUsage, _process.WorkingSet64);
            SaveUsageToFileAsync(model);
        }

        private async Task SaveUsageToFileAsync(ResourceUsageModel model)
        {
            await using (StreamWriter writer = File.AppendText(_tmpFilePath))
            {
                await writer.WriteLineAsync(model.ParseAsCsvLiteral());
            }
        }

        private void ProcessOnProcessStarted(object sender, ProcessStartedEventArgs e)
        {
            var currTime = DateTime.Now;
            _tmpFilePath = @$"{DirectoryHepler.GetTempPath()}/monitors/{currTime.FormatAsFileNameCode()}.csv";

            _timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            RecordProcessUsage();
        }

        public void Dispose()
        {
            
            _timer.Stop();
            _process?.Dispose();
            _timer?.Dispose();
            _performanceCounter.Dispose();
        }
    }
}