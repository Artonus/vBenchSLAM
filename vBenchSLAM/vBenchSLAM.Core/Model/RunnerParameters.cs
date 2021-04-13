using vBenchSLAM.Core.Enums;

namespace vBenchSLAM.Core.Model
{
    public class RunnerParameters
    {
        public string DatasetPath { get; set; }
        public string OutputPath { get; set; }
        public MapperType MapperType { get; set; }
        
        public RunnerParameters(MapperType mapperType, string outputPath, string datasetPath)
        {
            MapperType = mapperType;
            OutputPath = outputPath;
            DatasetPath = datasetPath;
        }
    }
}