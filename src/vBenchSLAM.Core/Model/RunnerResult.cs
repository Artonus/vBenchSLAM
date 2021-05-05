using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vBenchSLAM.Addins;

namespace vBenchSLAM.Core.Model
{
    public class RunnerResult
    {
        public bool IsSuccess { get; }
        public MapperType Mapper { get; }
        public string CreatedMapPath { get; }
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
