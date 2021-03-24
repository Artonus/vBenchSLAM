using System;
using System.Diagnostics;

namespace vBenchSLAM.Addins.Events
{
    public class ProcessRegisteredEventArgs : System.EventArgs
    {
        public Process Process { get; }
        public ProcessRegisteredEventArgs(Process process)
        {
            Process = process;
        }
    }
}