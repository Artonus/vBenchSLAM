using System;
using System.ComponentModel;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.Enums;
using vBenchSLAM.Core.Mappers;
using vBenchSLAM.Core.Mappers.Abstract;

namespace vBenchSLAM.Core
{
    public class Runner : IDisposable
    {
        private readonly MapperTypeEnum _mapperType;
        private readonly IDockerManager _dockerManager;
        private IMapper _mapper;
        public Runner(MapperTypeEnum mapperType, IDockerManager dockerManager)
        {
            _mapperType = mapperType;
            _dockerManager = dockerManager;
            CreateMapper(mapperType, dockerManager);
        }

        private void CreateMapper(MapperTypeEnum mapperType, IDockerManager dockerManager)
        {
            switch (mapperType)
            {
                case MapperTypeEnum.OpenVslam:
                    _mapper = new OpenVslamMapper(dockerManager);
                    break;
                default:
                    throw  new InvalidEnumArgumentException($"Unresolved mapper type: {mapperType}");
            }
        }

        public void Run()
        {
            _mapper.Start();
        }

        #region IDisposable implementation

        private void ReleaseUnmanagedResources()
        {
            _mapper = null;
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Runner()
        {
            ReleaseUnmanagedResources();
        }
        #endregion
    }
}
