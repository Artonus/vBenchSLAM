using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        protected BaseMapper(IDockerManager dockerManager, ILogger logger)
        {
            DockerManager = dockerManager;
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

        public abstract DatasetCheckResult ValidateDatasetCompleteness(RunnerParameters parameters);

        protected void SaveMapAndStatistics(DateTime started, DateTime finished, string resourceUsageFileName)
        {
            var parser = new BaseParser();
            var mapper = this as IMapper;
            string mapPath = Path.Combine(DirectoryHelper.GetDataFolderPath(), mapper?.MapFileName);
            var mapData = parser.GetMapDataFromMessagePack(mapPath);

            string documentsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "vBenchSLAM",
                Path.GetFileNameWithoutExtension(resourceUsageFileName));


            string currFileLocation = Path.Combine(DirectoryHelper.GetMonitorsPath(), resourceUsageFileName);
            string destinationLocation = Path.Combine(documentsPath, resourceUsageFileName);

            DirectoryHelper.CreateDirectoryIfNotExists(documentsPath);

            File.Copy(currFileLocation, destinationLocation);
            SaveMap(mapData, started, finished, documentsPath);
        }

        protected void SaveMap(ICsvParsable parsable, DateTime started, DateTime finished, string documentsPath)
        {
            string dataPath = Path.Combine(documentsPath, "data.csv");
            using (StreamWriter stream = File.CreateText(dataPath))
            {
                var mapper = this as IMapper;
                stream.WriteLine($"Started;Finished;Mapper");
                stream.WriteLine($"{started.ToLongTimeString()};{finished.ToLongTimeString()};{mapper.MapperType.GetStringValue()}");
                stream.WriteLine(parsable.GetCsvHeaderRow());
                stream.WriteLine(parsable.ParseAsCsvLiteral());
            }
        }
    }
}
