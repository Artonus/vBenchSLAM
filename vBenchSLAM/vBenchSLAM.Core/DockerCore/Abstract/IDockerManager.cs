using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vBenchSLAM.Core.DockerCore
{
    public interface IDockerManager
    {
        Task<IList<ContainerListResponse>> ListContainersAsync();
        Task<bool> StartContainerAsync(string container);
    }
}
