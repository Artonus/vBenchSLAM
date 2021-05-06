using System.Diagnostics;

namespace vBenchSLAM.Addins.Events
{
    /// <summary>
    /// Arguments for the custom process started event
    /// </summary>
    public class ProcessStartedEventArgs : System.EventArgs
    {
        public Process Process { get; }
        public ProcessStartedEventArgs(Process process)
        {
            Process = process;
        }
    }
}