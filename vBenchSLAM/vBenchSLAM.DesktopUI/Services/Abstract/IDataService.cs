using System.Collections.Generic;
using vBenchSLAM.DesktopUI.Models;

namespace vBenchSLAM.DesktopUI.Services
{
    public interface IDataService
    {
        IEnumerable<FrameworkModel> GetAvailableFrameworks();
    }
}