using System;
using System.Threading.Tasks;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.Enums;
using vBenchSLAM.Core.Mappers.Abstract;
using vBenchSLAM.Core.Mappers.Base;

namespace vBenchSLAM.Core.Mappers
{
    public class OpenVslamMapper : BaseMapper, IMapper
    {
        private readonly IDockerManager _dockerManager;
        private const string ServerContainerImage = "openvslam-server";
        private const string ViewerContainerImage = "";
        private const string FullName = "";

        public MapperTypeEnum MapperType => MapperTypeEnum.OpenVslam;
        public string FullFrameworkName => FullName;

        public OpenVslamMapper(IDockerManager dockerManager)
        {
            _dockerManager = dockerManager;
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
            return StartAsync().Result;
        }

        private async Task<bool> StartAsync()
        {
            var containerId = await _dockerManager.GetContainerIdByNameAsync(ServerContainerImage);
            var started = await _dockerManager.StartContainerAsync(containerId);
            var hasStarted = started;

            return hasStarted;
        }
        public bool Stop()
        {
            throw new NotImplementedException();
        }
    }
}
