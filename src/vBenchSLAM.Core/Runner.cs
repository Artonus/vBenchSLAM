﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Serilog;
using vBenchSLAM.Addins;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.Mappers;
using vBenchSLAM.Core.Mappers.Abstract;
using vBenchSLAM.Core.Mappers.DatasetServices;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.Core
{
    public class Runner : IRunner
    {
        /// <summary>
        /// Mapper instance
        /// </summary>
        private IMapper _mapper;
        /// <summary>
        /// Runner parameters selected by the user
        /// </summary>
        private readonly RunnerParameters _runnerParameters;
        /// <summary>
        /// Logger instance
        /// </summary>
        private readonly ILogger _logger;

        public Runner(RunnerParameters parameters)
        {
            _logger = Log.Logger;
            _runnerParameters = parameters;
            CreateMapper();
        }
        /// <summary>
        /// Creates the mapper based on the algorithm selected by the user
        /// </summary>
        private void CreateMapper()
        {
            switch (_runnerParameters.MapperType)
            {
                case MapperType.OpenVslam:
                    _mapper = new OpenVslamMapper(new ProcessRunner.ProcessRunner(),new OpenVslamDatasetService(_runnerParameters.DatasetType), _logger);
                    break;
                case MapperType.OrbSlam:
                    _mapper = new OrbSlamMapper(new ProcessRunner.ProcessRunner(), new OrbSlamDatasetService(_runnerParameters.DatasetType), _logger);
                    break;
                default:
                    throw  new InvalidEnumArgumentException($"Unresolved mapper type: {_runnerParameters.MapperType}");
            }
        }
        /// <summary>
        /// <inheritdoc cref="IRunner.Run"/>
        /// </summary>
        /// <returns></returns>
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
