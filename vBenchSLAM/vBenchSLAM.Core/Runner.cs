using System.ComponentModel;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.Enums;
using vBenchSLAM.Core.Mappers;
using vBenchSLAM.Core.Mappers.Abstract;

namespace vBenchSLAM.Core
{
    public class Runner
    {
        private readonly MapperTypeEnum _mapperType;
        private readonly IDockerManager _dockerManager;
        private IMapper _mapper;
        public Runner(MapperTypeEnum mapperType, IDockerManager dockerManager)
        {
            _mapperType = mapperType;
            _dockerManager = dockerManager;
            CreateMapper();
        }

        private void CreateMapper()
        {
            switch (_mapperType)
            {
                case MapperTypeEnum.OpenVslam:
                    _mapper = new OpenVslamMapper(_dockerManager);
                    break;
                default:
                    throw  new InvalidEnumArgumentException($"Unresolved mapper type: {_mapperType}");
            }
        }

        public void Run()
        {
            _mapper.Start();
        }
    }
}
