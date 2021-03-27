using System;
using System.ComponentModel;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.Enums;
using vBenchSLAM.Core.Mappers;
using vBenchSLAM.Core.Mappers.Abstract;
using vBenchSLAM.Core.Model;
using vBenchSLAM.Core.ProcessRunner;

namespace vBenchSLAM.Core
{
    public class Runner : IDisposable
    {
        private IMapper _mapper;
        private readonly RunnerParameters _runnerParameters;

        public Runner(RunnerParameters parameters)
        {
            _runnerParameters = parameters;
        }

        private void CreateMapper()
        {
            switch (_runnerParameters.MapperType)
            {
                case MapperTypeEnum.OpenVslam:
                    _mapper = new OpenVslamMapper(new DockerManager(new OpenVslamProcessRunner()));
                    break;
                default:
                    throw  new InvalidEnumArgumentException($"Unresolved mapper type: {_runnerParameters.MapperType}");
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
