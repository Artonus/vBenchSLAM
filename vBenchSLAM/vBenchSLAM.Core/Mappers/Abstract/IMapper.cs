using System.Threading.Tasks;
using vBenchSLAM.Core.Enums;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.Core.Mappers.Abstract
{
    public interface IMapper
    {
        MapperType MapperType { get; }

        string MapFileName { get; }

        Task<bool> Map();
        
        DatasetCheckResult ValidateDatasetCompleteness(RunnerParameters parameters);
        void CopyMapToOutputFolder(string outputFolder);
    }
}
