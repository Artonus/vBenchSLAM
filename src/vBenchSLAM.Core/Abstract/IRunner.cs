using System;
using System.Threading.Tasks;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.Core
{
    public interface IRunner : IDisposable
    {
        /// <summary>
        /// Run the mapping algorithm
        /// </summary>
        /// <returns></returns>
        Task<RunnerResult> Run();
    }
}