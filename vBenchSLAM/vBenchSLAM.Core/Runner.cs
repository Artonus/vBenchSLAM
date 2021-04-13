using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Serilog;
using vBenchSLAM.Addins;
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
        private readonly ILogger _logger;

        public Runner(RunnerParameters parameters)
        {
            _logger = Log.Logger;
            _runnerParameters = parameters;
            CreateMapper();
        }

        private void CreateMapper()
        {
            switch (_runnerParameters.MapperType)
            {
                case MapperType.OpenVslam:
                    _mapper = new OpenVslamMapper(new DockerManager(new OpenVslamProcessRunner()), _logger);
                    break;
                default:
                    throw  new InvalidEnumArgumentException($"Unresolved mapper type: {_runnerParameters.MapperType}");
            }
        }

        public async Task<RunnerResult> Run()
        {
            DatasetCheckResult checkResult = null;
            try
            {
                checkResult = _mapper.ValidateDatasetCompleteness(_runnerParameters);
                if (checkResult.IsValid == false)
                {
                    return new RunnerResult(false, _runnerParameters.MapperType, string.Empty,
                        checkResult.Exception);
                }

                await _mapper.Map();
            }
            catch (Exception ex)
            {
                return new RunnerResult(false, _runnerParameters.MapperType, string.Empty, ex);
            }
            finally
            {
                if (checkResult is not null && checkResult.IsValid)
                    _mapper.CopyMapToOutputFolder(_runnerParameters.OutputPath);
             
                
                DirectoryHelper.ClearDataFolder();
            }
            return new RunnerResult(true, _runnerParameters.MapperType, string.Empty, null);
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
