using System;
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
        private readonly Action<ProcessMonitor> _removeFromRegistryAction;
        private readonly VBenchProcess _process;
        private readonly Timer _timer;
        private string _tmpFilePath;
        private decimal _prevCpuUsageTime;
        private DateTime _prevTimeCheck;

        public ProcessMonitor(VBenchProcess process, Action<ProcessMonitor> removeFromRegistryAction)
        {
            _removeFromRegistryAction = removeFromRegistryAction;
            _process = process;
            process.ProcessStarted += ProcessOnProcessStarted;
            process.Exited += ProcessOnExited;
            _timer = new Timer
            {
                Interval = 2000
            };
            _timer.Elapsed += TimerOnElapsed;
        }

        private async Task RecordProcessUsage()
        {
            var currTime = DateTime.Now;
            var currCpuUsageTime = (decimal) _process.TotalProcessorTime.TotalMilliseconds;

            if (_prevTimeCheck != default)
            {
                var timePassed = (decimal) (currTime - _prevTimeCheck).TotalMilliseconds;
                var currCpuMs = (currCpuUsageTime - _prevCpuUsageTime);
                var procCpuUsage = currCpuMs * 100 / (Environment.ProcessorCount * timePassed);
                var model = new ResourceUsageModel(procCpuUsage, _process.WorkingSet64);
                await SaveUsageToFileAsync(model);
            }

            _prevTimeCheck = currTime;
            _prevCpuUsageTime = currCpuUsageTime;
        }

        private async Task SaveUsageToFileAsync(ResourceUsageModel model)
        {
            try
            {
                var fInfo = new FileInfo(_tmpFilePath);
                if (fInfo.Exists == false)
                {
                    if (string.IsNullOrEmpty(fInfo.DirectoryName) == false 
                        && Directory.Exists(fInfo.DirectoryName) == false)
                    {
                        Directory.CreateDirectory(fInfo.DirectoryName);
                    }
                }

                await using (StreamWriter writer = fInfo.Exists
                    ? File.AppendText(fInfo.FullName)
                    : File.CreateText(fInfo.FullName))
                {
                    await writer.WriteLineAsync(model.ParseAsCsvLiteral());
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Could not access the file: {_tmpFilePath}, error: {ex}");
            }
        }

        private void ProcessOnProcessStarted(object sender, ProcessStartedEventArgs e)
        {
            // left just in case
            // _performanceCounter = new PerformanceCounter("Process", "% Processor Time",
            //     e.Process.ProcessName, true);
            var currTime = DateTime.Now;
            var tmpPath = DirectoryHelper.GetTempPath();
            _tmpFilePath = @$"{tmpPath}monitors/{currTime.FormatAsFileNameCode()}.csv";

            _timer.Start();
        }

        private void ProcessOnExited(object sender, EventArgs e)
        {
            _removeFromRegistryAction?.Invoke(this);
        }

        private async void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            await RecordProcessUsage();
        }

        public void Dispose()
        {
            _timer.Stop();
            _process?.Dispose();
            _timer?.Dispose();
        }
    }
}