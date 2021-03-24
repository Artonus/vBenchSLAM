using System.Diagnostics;

namespace vBenchSLAM.Addins.Events
{
    public class ProcessStartedEventArgs : System.EventArgs
    {
        public Process Process { get; }
        public ProcessStartedEventArgs(Process process)
        {
            Process = process;
        }
    }
}