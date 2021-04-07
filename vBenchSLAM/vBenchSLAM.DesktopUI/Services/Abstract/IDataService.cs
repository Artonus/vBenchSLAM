using System.Collections.Generic;
using vBenchSLAM.DesktopUI.Models;

namespace vBenchSLAM.DesktopUI.Services
{
    public interface IDataService
    {
        /// <summary>
        /// Gets the collection of all currently available frameworks 
        /// </summary>
        /// <returns></returns>
        IEnumerable<FrameworkModel> GetAvailableFrameworks();
        /// <summary>
        /// Get runs list in the form of the strings representing the moment when the run has started
        /// </summary>
        /// <returns></returns>
        List<string> GetRunLog();
        /// <summary>
        /// Parses the values to the form that can be presented on the chart
        /// </summary>
        /// <param name="run">run identifier in the form of the stringed date</param>
        /// <returns></returns>
        ChartDataModel GetRunDataForChart(string run);
    }
}