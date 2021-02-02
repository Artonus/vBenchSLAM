using System.Diagnostics;
using vBenchSLAM.Addins.EventArgs;

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
                OnProcessStarted(new ProcessStartedEventArgs());
            }

            return started;
        }

        protected virtual void OnProcessStarted(ProcessStartedEventArgs e)
        {
            ProcessStarted?.Invoke(this, e);
        }
    }
}