using vBenchSLAM.Core.Enums;

namespace vBenchSLAM.Core.Mappers.Abstract
{
    public interface IMapper
    {
        MapperTypeEnum MapperType { get; }
        string FullFrameworkName { get; }
        string SaveMap();
        bool ShowMap();
        bool Start();
        bool Stop();
        

    }
}
