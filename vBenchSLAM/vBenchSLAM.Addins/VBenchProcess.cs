using System.Diagnostics;
using vBenchSLAM.Addins.Events;

namespace vBenchSLAM.Addins
{
    public class VBenchProcess : Process
    {
        public delegate void ProcessStartedEventHandler(object sender, ProcessStartedEventArgs e);

        public event ProcessStartedEventHandler ProcessStarted;

        public new bool Start()
        {
            var started = base.Start();
            if (started)
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