using vBenchSLAM.Addins;

namespace vBenchSLAM.Core.Model
{
    public class RunnerParameters
    {
        public string DatasetPath { get; }
        public string OutputPath { get; }
        public MapperType MapperType { get; }
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