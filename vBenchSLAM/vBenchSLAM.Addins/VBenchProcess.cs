using System.Diagnostics;
using vBenchSLAM.Addins.Events;

namespace vBenchSLAM.Addins
{
    public class VBenchProcess : Process
    {
        public bool EnableRaisingCustomEvents { get; set; }

        public delegate void ProcessStartedEventHandler(object sender, ProcessStartedEventArgs e);

        public event ProcessStartedEventHandler ProcessStarted;

        public VBenchProcess(ProcessStartInfo startInfo, bool enableRaisingCustomEvents)
        {
            EnableRaisingCustomEvents = enableRaisingCustomEvents;
            StartInfo = startInfo;
            EnableRaisingEvents = true;
        }

        public new bool Start()
        {
            var started = base.Start();
            if (started && EnableRaisingCustomEvents)
            {
                OnProcessStarted(new ProcessStartedEventArgs(this));
            }
            return started;
        }

        private void OnProcessStarted(ProcessStartedEventArgs e)
        {
            ProcessStarted?.Invoke(this, e);
        }
    }
}