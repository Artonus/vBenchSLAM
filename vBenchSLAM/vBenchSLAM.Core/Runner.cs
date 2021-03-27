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
            CreateMapper();
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

        public RunnerResultModel Run()
        {
            try
            {
                var checkResult = _mapper.ValidateDatasetCompleteness(_runnerParameters);
                if (checkResult.IsValid == false)
                {
                    return new RunnerResultModel(false, _runnerParameters.MapperType, string.Empty,
                        checkResult.Exception);
                }

                _mapper.Map();

                return new RunnerResultModel(true, _runnerParameters.MapperType, string.Empty, null);
            }
            catch (Exception ex)
            {
                return new RunnerResultModel(false, _runnerParameters.MapperType, string.Empty, ex);
            }
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
