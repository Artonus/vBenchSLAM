using System.Threading.Tasks;
using vBenchSLAM.Addins;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.Core.Mappers.Abstract
{
    internal interface IMapper
    {
        /// <summary>
        /// The mapping framework used by the current mapper instance
        /// </summary>
        MapperType MapperType { get; }
        /// <summary>
        /// Name of the data file outputted by the SLAM algorithm
        /// </summary>

        string MapFileName { get; }
        /// <summary>
        /// Start the mapping using the selected algorithm
        /// </summary>
        /// <returns><see cref="true"/> if the algorithm has run correctly, otherwise false</returns>
        Task<bool> Map();
        /// <summary>
        /// Validates if all files needed by the algorithm are present in the directory selected by a user
        /// </summary>
        /// <param name="parameters">Parameters selected by a user in main window</param>
        /// <returns></returns>
        DatasetCheckResult ValidateDatasetCompleteness(RunnerParameters parameters);
        /// <summary>
        /// Copies the map created by the algorithm to the output folder selected by a user
        /// </summary>
        /// <param name="outputFolder"></param>
        void CopyMapToOutputFolder(string outputFolder);
    }
}
