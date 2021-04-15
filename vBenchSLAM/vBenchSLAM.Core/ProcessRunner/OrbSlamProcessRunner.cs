using System.Threading.Tasks;

namespace vBenchSLAM.Core.ProcessRunner
{
    public class OrbSlamProcessRunner : ProcessRunner
    {
        public async Task EnablePangolinViewer()
        {
            await RunProcessAsync(BaseProgram, $"{ExecCmdOption} \"xhost + \"", false);
        }
    }
}