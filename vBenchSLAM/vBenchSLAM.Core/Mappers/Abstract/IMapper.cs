using vBenchSLAM.Core.Enums;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.Core.Mappers.Abstract
{
    public interface IMapper
    {
        MapperTypeEnum MapperType { get; }

        string MapFileName { get; }

        bool Map();
        
        DatasetCheckResult ValidateDatasetCompleteness(RunnerParameters parameters);
        void CopyMapToOutputFolder(string outputFolder);
    }
}
