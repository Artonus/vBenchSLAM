using System.IO;
using System;
using System.Linq;
using vBenchSLAM.Addins;
using vBenchSLAM.Core.Mappers.Abstract;
using vBenchSLAM.Core.Model;

namespace vBenchSLAM.Core.Mappers.DatasetServices
{
    /// <summary>
    /// Dataset service for the OpenVSLAM mapper
    /// </summary>
    internal class OpenVslamDatasetService : IDatasetService
    {
        /// <summary>
        /// <inheritdoc cref="IDatasetService.DatasetType"/>
        /// </summary>
        public DatasetType DatasetType { get; init; }

        public OpenVslamDatasetService(DatasetType datasetType)
        {
            DatasetType = datasetType;
        }
        /// <summary>
        /// <inheritdoc cref="IDatasetService.ValidateDatasetCompleteness"/>
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DatasetCheckResult ValidateDatasetCompleteness(RunnerParameters parameters)
        {
            string vocabFileName = "orb_vocab_openvslam.dbow2", configFileName = "config_openvslam.yaml", sequenceFolderName = "sequence";

            var allFiles = Directory.GetFiles(parameters.DatasetPath);
            var fileInfos = allFiles.Select(path => new FileInfo(path)).ToList();

            var vocabFile = fileInfos.SingleOrDefault(f => f.Extension == ".dbow2" && f.Name == vocabFileName);
            if (vocabFile is null || vocabFile.Exists == false)
            {
                return new DatasetCheckResult(false,
                    new Exception($"Cannot find the vocabulary file: {vocabFileName}"));
            }

            var configFile = fileInfos.SingleOrDefault(f => f.Extension == ".yaml" && f.Name == configFileName);
            if (configFile is null || configFile.Exists == false)
            {
                return new DatasetCheckResult(false,
                    new Exception($"Cannot find the configuration file: {configFileName}"));
            }

            DirectoryInfo sequencePath = default;
            sequencePath = new DirectoryInfo(Path.Combine(parameters.DatasetPath, sequenceFolderName));
            if (sequencePath.Exists == false)
            {
                return new DatasetCheckResult(false,
                    new Exception($"Cannot find the sequence folder"));
            }

            var retVal = new DatasetCheckResult(true, null)
            {
                VocabFile = vocabFile,
                ConfigFile = configFile,
                SequenceDirectory = sequencePath
            };
            return retVal;
        }
    }
}