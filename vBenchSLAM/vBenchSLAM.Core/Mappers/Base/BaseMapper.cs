using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.Core.Mappers.Base
{
    public abstract class BaseMapper
    {
        protected string RunParameters;
        protected readonly IDockerManager DockerManager;

        protected BaseMapper(IDockerManager dockerManager)
        {
            DockerManager = dockerManager;
        }

        public static string GetFullImageName(string image)
        {
            return $"{Settings.VBenchSlamRepositoryName}:{image}";
        }

        protected virtual async Task<bool> ParallelStopContainersAsync(params string[] containerNames)
        {
            var stopped = new List<Task<bool>>();
            foreach (var container in containerNames)
            {
                stopped.Add(FindAndStopContainerAsync(container));
            }

            var results = await Task.WhenAll(stopped);
            return results.All(r => r);
        }

        protected virtual async Task<bool> FindAndStopContainerAsync(string containerName)
        {
            var container = await DockerManager.GetContainerByNameAsync(GetFullImageName(containerName));
            return await DockerManager.StopContainerAsync(container.ID);
        }

        public abstract DatasetCheckResult ValidateDatasetCompleteness(RunnerParameters parameters);
    }
}
