using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vBenchSLAM.Core.Mappers.Base
{
    public abstract class BaseMapper
    {
        protected string _runParameters;
        protected BaseMapper()
        {
                
        }

        protected static string GetFullImageName(string repository, string tag)
        {
            return $"{repository}:{tag}";
        }
    }
}
