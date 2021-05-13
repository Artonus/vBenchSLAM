using System;
using System.Diagnostics;

namespace vBenchSLAM.Addins.Events
{
    /// <summary>
    /// Arguments for the custom process registered event
    /// </summary>
    public class ProcessRegisteredEventArgs : System.EventArgs
    {
        public Process Process { get; }
        public ProcessRegisteredEventArgs(Process process)
        {
            Process = process;
        }
    }
}