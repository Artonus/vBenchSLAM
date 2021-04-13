using System.Threading.Tasks;
using Serilog;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.Enums;
using vBenchSLAM.Core.Mappers.Abstract;
using vBenchSLAM.Core.Mappers.Base;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.Core.Mappers
{
    public class OrbSlamMapper : BaseMapper, IMapper
    {
        public OrbSlamMapper(IDockerManager dockerManager, ILogger logger) : base(dockerManager, logger)
        {
            throw new System.NotImplementedException();
        }

        public MapperType MapperType => MapperType.OrbSlam;
        public string MapFileName => "KeyFrameTrajectory.txt";
        public Task<bool> Map()
        {
            throw new System.NotImplementedException();
        }

        public override DatasetCheckResult ValidateDatasetCompleteness(RunnerParameters parameters)
        {
            throw new System.NotImplementedException();
        }

        public void CopyMapToOutputFolder(string outputFolder)
        {
            throw new System.NotImplementedException();
        }
    }
}