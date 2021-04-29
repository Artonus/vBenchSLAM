using System;
using System.Threading.Tasks;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.Core
{
    public interface IRunner : IDisposable
    {
        Task<RunnerResult> Run();
    }
}