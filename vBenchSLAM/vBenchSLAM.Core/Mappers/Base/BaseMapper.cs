using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Serilog;
using vBenchSLAM.Addins;
using vBenchSLAM.Addins.ExtensionMethods;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.MapParser;
using vBenchSLAM.Core.Mappers.Abstract;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.Core.Mappers.Base
{
    public abstract class BaseMapper
    {
        protected string RunParameters;
        protected readonly IDockerManager DockerManager;
        
        protected readonly ILogger Logger;
        protected BaseParser Parser;

        protected BaseMapper(ProcessRunner.ProcessRunner processRunner, ILogger logger)
        {
            DockerManager = new DockerManager(processRunner);
            Logger = logger;
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
        
        public abstract string GetContainerCommand();
        public abstract DatasetCheckResult ValidateDatasetCompleteness(RunnerParameters parameters);
        

        protected void SaveMapAndStatistics(DateTime started, DateTime finished, string resourceUsageFileName)
        {
            var mapper = this as IMapper;
            string mapPath = Path.Combine(DirectoryHelper.GetDataFolderPath(), mapper?.MapFileName ?? throw new InvalidOperationException());
            var mapData = Parser.ParseMap(mapPath);

            string documentsPath = Path.Combine(
               DirectoryHelper.GetUserDocumentsFolder(),
               Path.GetFileNameWithoutExtension(resourceUsageFileName));

            string currFileLocation = Path.Combine(DirectoryHelper.GetMonitorsPath(), resourceUsageFileName);
            string destinationLocation = Path.Combine(documentsPath, resourceUsageFileName);

            DirectoryHelper.CreateDirectoryIfNotExists(documentsPath);

            File.Copy(currFileLocation, destinationLocation);
            SaveMap(mapData, started, finished, documentsPath);
            LogRun(resourceUsageFileName);
        }

        protected void SaveMap(ICsvParsable parsable, DateTime started, DateTime finished, string documentsPath)
        {
            string dataPath = Path.Combine(documentsPath, Settings.RunDataFileName);
            using (StreamWriter stream = File.CreateText(dataPath))
            {
                var mapper = this as IMapper;
                stream.WriteLine($"Started;Finished;Mapper");
                stream.WriteLine($"{started.ToString(CultureInfo.InvariantCulture)};{finished.ToString(CultureInfo.InvariantCulture)};{mapper.MapperType.GetStringValue()}");
                stream.WriteLine(parsable.GetCsvHeaderRow());
                stream.WriteLine(parsable.ParseAsCsvLiteral());
            }
        }
        
        private void LogRun(string resourceUsageFileName)
        {
            var logFile = new FileInfo(Path.Combine(DirectoryHelper.GetUserDocumentsFolder(), Settings.RunLogFileName));
            using (StreamWriter writer = logFile.Exists
                ? File.AppendText(logFile.FullName)
                : File.CreateText(logFile.FullName))
            {
                writer.WriteLine(Path.GetFileNameWithoutExtension(resourceUsageFileName));
            }
        }

        protected void CopyToTemporaryFilesFolder(params FileInfo[] fileInfos)
        {
            var tempFolderPath = DirectoryHelper.GetDataFolderPath();
            foreach (var file in fileInfos)
            {
                if (file is null)
                    continue;

                var copiedFileDestination = Path.Combine(tempFolderPath, file.Name);
                FileHelper.SafeCopy(file.FullName, copiedFileDestination);
            }
        }

        protected void CopySequenceFolder(DirectoryInfo sequenceFolderName)
        {
            var destSequenceFolderPath =new DirectoryInfo(Path.Combine(DirectoryHelper.GetDataFolderPath(), sequenceFolderName.Name));
            DirectoryHelper.CopyFullDir(sequenceFolderName, destSequenceFolderPath);
        }

        public void CopyMapToOutputFolder(string outputFolder, string mapFileName)
        {
            string mapFile = Path.Combine(DirectoryHelper.GetDataFolderPath(), mapFileName);
            string destFileName = Path.Combine(outputFolder, mapFileName);
            FileHelper.SafeCopy(mapFile, destFileName);
        }
    }
}
