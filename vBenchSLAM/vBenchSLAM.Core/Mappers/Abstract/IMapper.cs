using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vBenchSLAM.Core.Mappers.Abstract
{
    public interface IMapper
    {
        string FullFrameworkName { get; }
        string SaveMap();
        bool ShowMap();
        bool Start();
        bool Stop();
        

    }
}
