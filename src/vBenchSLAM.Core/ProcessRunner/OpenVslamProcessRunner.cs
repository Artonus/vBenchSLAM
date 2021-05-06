using System.IO;
using System.Threading.Tasks;
using vBenchSLAM.Core.Mappers;
using vBenchSLAM.Core.Mappers.Base;

namespace vBenchSLAM.Core.ProcessRunner
{
    public class OpenVslamProcessRunner : ProcessRunner
    {
        /// <inheritdoc />
        public override async Task<int> StartContainerViaCommandLineAsync(string containerName, string startParameters,
            string containerCommand = "")
        {
            if (containerName != BaseMapper.GetFullImageName(OpenVslamMapper.ViewerContainerImage))
            {
                return await base.StartContainerViaCommandLineAsync(containerName, startParameters, containerCommand);
            }
             
            var fInfo = await CreateScriptFile(containerName, startParameters, containerCommand);

            var result = await RunProcessAsync(BaseProgram, $"{ExecCmdOption} ./{fInfo.Name}");

            File.Delete(fInfo.FullName);

            return result;
        }

        private async Task<FileInfo> CreateScriptFile(string containerName, string startParameters, string containerCommand)
        {
            var fInfo = new FileInfo("run.sh");
            if (fInfo.Exists)
            {
                File.Delete(fInfo.FullName);
            }

            using (var sw = File.CreateText(fInfo.FullName))
            {
                sw.WriteLine("#!/bin/bash");
                var cmd = GetDockerRunCommand(containerName, startParameters, containerCommand);
                sw.WriteLine($"echo 'executing command: {cmd}'");
                sw.WriteLine(cmd);
            }

            await SetAsExecutable(fInfo);
            return fInfo;
        }

        private async Task SetAsExecutable(FileInfo fInfo)
        {
            await RunProcessAsync(BaseProgram, $"{ExecCmdOption} \"chmod +x {fInfo.Name}\"", false);

            #region DoesntWorkOnLinux

            // var fs = new FileSecurity(fInfo.FullName, AccessControlSections.All);
            // var securityId = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null).Translate(typeof(NTAccount));
            // var accessRule =
            //     new FileSystemAccessRule(securityId, FileSystemRights.ExecuteFile, AccessControlType.Allow);
            // fs.AddAccessRule(accessRule);

            #endregion
        }
    }
}