using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vBenchSLAM.Core.Enums;

namespace vBenchSLAM.Core.Model
{
    public class RunnerResultModel
    {
        public bool IsSuccess { get; }
        public MapperTypeEnum Mapper { get; }
        public string CreatedMapPath { get; }
        public Exception Exception { get; }

        public RunnerResultModel(bool isSuccess, MapperTypeEnum mapper, string createdMapPath, Exception exception)
        {
            IsSuccess = isSuccess;
            Mapper = mapper;
            CreatedMapPath = createdMapPath;
            Exception = exception;
        }
    }
}
