using vBenchSLAM.Core.Enums;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.Core.Mappers.Abstract
{
    public interface IMapper
    {
        MapperTypeEnum MapperType { get; }
        string FullFrameworkName { get; }
        string SaveMap();
        bool ShowMap();
        bool Map();
        bool Stop();
        DatasetCheckResult ValidateDatasetCompleteness(RunnerParameters parameters);
    }
}
