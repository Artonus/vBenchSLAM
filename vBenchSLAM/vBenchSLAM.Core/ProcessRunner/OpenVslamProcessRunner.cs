using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
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
            //TODO: set file permissions and test
            //SetAsExecutable(fInfo);

            var result = await base.RunProcessAsync(BaseProgram, $"-c ./{fInfo.Name}");

            File.Delete(fInfo.FullName);

            return result;
        }

        private void SetAsExecutable(FileInfo fInfo)
        {
            var fs = new FileSecurity(fInfo.FullName, AccessControlSections.All);
            var securityId = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null).Translate(typeof(NTAccount));
            var accessRule =
                new FileSystemAccessRule(securityId, FileSystemRights.ExecuteFile, AccessControlType.Allow);
            fs.AddAccessRule(accessRule);
        }
    }
}