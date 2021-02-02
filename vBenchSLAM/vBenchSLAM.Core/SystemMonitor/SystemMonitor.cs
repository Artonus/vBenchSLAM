using System;
using System.ComponentModel;
using System.Diagnostics;
using vBenchSLAM.Addins;
using vBenchSLAM.Addins.EventArgs;

namespace vBenchSLAM.Core.SystemMonitor
{
    public class SystemMonitor : IDisposable
    {
        private readonly Process _process;

        public SystemMonitor(VBenchProcess process)
        {
            _process = process;
            process.ProcessStarted += ProcessOnProcessStarted;
            
            //_timer = new Timer    
        }

        private void ProcessOnProcessStarted(object sender, ProcessStartedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _process?.Dispose();
        }
        
    }
}