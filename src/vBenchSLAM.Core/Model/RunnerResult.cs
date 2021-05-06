using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vBenchSLAM.Addins;

namespace vBenchSLAM.Core.Model
{
    /// <summary>
    /// Represents the result of the running mapper algorithm
    /// </summary>
    public class RunnerResult
    {
        /// <summary>
        /// Run has been successful 
        /// </summary>
        public bool IsSuccess { get; }
        /// <summary>
        /// Used mapper type
        /// </summary>
        public MapperType Mapper { get; }
        /// <summary>
        /// Path to the created map
        /// </summary>
        public string CreatedMapPath { get; }
        /// <summary>
        /// Exception that occurred. Null if the run was successful
        /// </summary>
        public Exception Exception { get; }

        public RunnerResult(bool isSuccess, MapperType mapper, string createdMapPath, Exception exception)
        {
            IsSuccess = isSuccess;
            Mapper = mapper;
            CreatedMapPath = createdMapPath;
            Exception = exception;
        }
    }
}
