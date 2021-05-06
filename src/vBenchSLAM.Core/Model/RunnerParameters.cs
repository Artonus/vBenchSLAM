using vBenchSLAM.Addins;

namespace vBenchSLAM.Core.Model
{
    public class RunnerParameters
    {
        /// <summary>
        /// Selected path to the dataset
        /// </summary>
        public string DatasetPath { get; }
        /// <summary>
        /// Selected output path
        /// </summary>
        public string OutputPath { get; }
        /// <summary>
        /// Selected mapper type
        /// </summary>
        public MapperType MapperType { get; }
        /// <summary>
        /// Selected dataset type
        /// </summary>
        public DatasetType DatasetType { get; }

        public RunnerParameters(MapperType mapperType, DatasetType datasetType, string outputPath, string datasetPath)
        {
            MapperType = mapperType;
            DatasetType = datasetType;
            OutputPath = outputPath;
            DatasetPath = datasetPath;
        }
    }
}