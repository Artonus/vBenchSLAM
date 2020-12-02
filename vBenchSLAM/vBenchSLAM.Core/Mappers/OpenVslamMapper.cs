using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vBenchSLAM.Core.Mappers.Abstract;
using vBenchSLAM.Core.Mappers.Base;

namespace vBenchSLAM.Core.Mappers
{
    public class OpenVslamMapper : BaseMapper, IMapper
    {
        private const string FullName = "";

        public string FullFrameworkName => FullName;

        public OpenVslamMapper()
        {
            
        }
        public string SaveMap()
        {
            throw new NotImplementedException();
        }

        public bool ShowMap()
        {
            throw new NotImplementedException();
        }
        public bool Start()
        {
            throw new NotImplementedException();
        }
        public bool Stop()
        {
            throw new NotImplementedException();
        }
    }
}
