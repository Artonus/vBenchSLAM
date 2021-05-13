using vBenchSLAM.Addins;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.Core.Mappers.Abstract
{
    internal interface IDatasetService
    {
        /// <summary>
        /// Represents the active dataset type
        /// </summary>
        DatasetType DatasetType { get; }
        /// <summary>
        /// Checks if the selected dataset consists of all the necessary files
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        DatasetCheckResult ValidateDatasetCompleteness(RunnerParameters parameters);
    }
}