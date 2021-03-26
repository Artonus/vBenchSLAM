using System;
using System.ComponentModel;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.Enums;
using vBenchSLAM.Core.Mappers;
using vBenchSLAM.Core.Mappers.Abstract;
using vBenchSLAM.Core.ProcessRunner;

namespace vBenchSLAM.Core
{
    public class Runner : IDisposable
    {
        private readonly MapperTypeEnum _mapperType;
        private IMapper _mapper;
        public Runner(MapperTypeEnum mapperType)
        {
            _mapperType = mapperType;
            CreateMapper();
        }

        private void CreateMapper()
        {
            switch (_mapperType)
            {
                case MapperTypeEnum.OpenVslam:
                    _mapper = new OpenVslamMapper(new DockerManager(new OpenVslamProcessRunner()));
                    break;
                default:
                    throw  new InvalidEnumArgumentException($"Unresolved mapper type: {_mapperType}");
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
