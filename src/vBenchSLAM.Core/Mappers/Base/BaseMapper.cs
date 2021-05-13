using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using vBenchSLAM.Addins;
using vBenchSLAM.Addins.Abstract;
using vBenchSLAM.Addins.ExtensionMethods;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.MapParser;
using vBenchSLAM.Core.Mappers.Abstract;
using vBenchSLAM.Core.Model;
using vBenchSLAM.Core.ProcessRunner;

namespace vBenchSLAM.Core.Mappers.Base
{
    /// <summary>
    /// Base class for the mappers
    /// </summary>
    internal abstract class BaseMapper
    {
        /// <summary>
        /// Instance of DockerManager
        /// </summary>
        protected readonly IDockerManager DockerManager;
        /// <summary>
        /// Process runner instance
        /// </summary>
        protected readonly IProcessRunner ProcessRunner;
        /// <summary>
        /// Logger instance
        /// </summary>
        protected readonly ILogger Logger;
        /// <summary>
        /// Map parser instance
        /// </summary>
        protected BaseParser Parser;

        protected BaseMapper(IProcessRunner processRunner, ILogger logger)
        {
            ProcessRunner = processRunner;
            DockerManager = new DockerManager(processRunner);
            Logger = logger;
        }
        /// <summary>
        /// Get full image name to be downloaded by the Docker
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string GetFullImageName(string image)
        {
            return $"{Settings.VBenchSlamRepositoryName}:{image}";
        }
        /// <summary>
        /// Asynchronously stops containers in parallel
        /// </summary>
        /// <param name="containerNames"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Asynchronously finds and stops the container by it's name
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        protected virtual async Task<bool> FindAndStopContainerAsync(string containerName)
        {
            var container = await DockerManager.GetContainerByNameAsync(GetFullImageName(containerName));
            return await DockerManager.StopContainerAsync(container.ID);
        }
        /// <summary>
        /// Returns the command that will be used by the container to run the algorithm
        /// </summary>
        /// <returns></returns>
        public abstract string GetContainerCommand();
        /// <summary>
        /// <inheritdoc cref="IMapper.ValidateDatasetCompleteness"/>
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public abstract DatasetCheckResult ValidateDatasetCompleteness(RunnerParameters parameters);

        /// <summary>
        /// Saves the created map and run data to the appropriate directory, confirms successful run of the algorithm
        /// </summary>
        /// <param name="started"></param>
        /// <param name="finished"></param>
        /// <param name="resourceUsageFileName"></param>
        protected void ConfirmRunFinished(DateTime started, DateTime finished, string resourceUsageFileName)
        {
            var mapper = this as IMapper;
            string mapPath = Path.Combine(DirectoryHelper.GetDataFolderPath(), mapper?.MapFileName ?? throw new InvalidOperationException());
            var mapData = Parser.ParseMap(mapPath);

            string documentsPath = Path.Combine(
               DirectoryHelper.GetUserDocumentsFolder(),
               Path.GetFileNameWithoutExtension(resourceUsageFileName));

            string currFileLocation = Path.Combine(DirectoryHelper.GetResourceMonitorsPath(), resourceUsageFileName);
            string destinationLocation = Path.Combine(documentsPath, resourceUsageFileName);

            DirectoryHelper.CreateDirectoryIfNotExists(documentsPath);

            File.Copy(currFileLocation, destinationLocation);
            SaveMap(mapData, started, finished, documentsPath);
            LogRun(resourceUsageFileName);
        }
        /// <summary>
        /// Saves the map data to the destination directory
        /// </summary>
        /// <param name="parsable"></param>
        /// <param name="started"></param>
        /// <param name="finished"></param>
        /// <param name="documentsPath"></param>
        private void SaveMap(ICsvParsable parsable, DateTime started, DateTime finished, string documentsPath)
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
        /// <summary>
        /// Confirms the algorithm has successfully finished running
        /// </summary>
        /// <param name="resourceUsageFileName"></param>
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
        /// <summary>
        /// Copies files to the temporary data directory folder
        /// </summary>
        /// <param name="fileInfos"></param>
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
        /// <summary>
        /// Copies the folder containing the sequence from the dataset to the temporary data directory
        /// </summary>
        /// <param name="sequenceFolderName"></param>
        protected void CopySequenceFolder(DirectoryInfo sequenceFolderName)
        {
            var destSequenceFolderPath = new DirectoryInfo(Path.Combine(DirectoryHelper.GetDataFolderPath(), sequenceFolderName.Name));
            DirectoryHelper.CopyFullDir(sequenceFolderName, destSequenceFolderPath);
        }
        /// <summary>
        /// Copies the map created by the algorithm to the output folder specified by the user
        /// </summary>
        /// <param name="outputFolder"></param>
        /// <param name="mapFileName"></param>
        protected void CopyMapToOutputFolder(string outputFolder, string mapFileName)
        {
            string mapFile = Path.Combine(DirectoryHelper.GetDataFolderPath(), mapFileName);
            string destFileName = Path.Combine(outputFolder, mapFileName);
            FileHelper.SafeCopy(mapFile, destFileName);
        }
    }
}
