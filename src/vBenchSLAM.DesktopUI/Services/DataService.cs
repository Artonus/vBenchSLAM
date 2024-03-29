﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using vBenchSLAM.Addins;
using vBenchSLAM.Addins.Models;
using vBenchSLAM.Core;
using vBenchSLAM.DesktopUI.Models;

namespace vBenchSLAM.DesktopUI.Services
{
    /// <summary>
    /// <inheritdoc cref="IDataService"/>
    /// </summary>
    public class DataService : IDataService
    {
        /// <summary>
        /// <inheritdoc cref="IDataService.GetAvailableFrameworks"/>
        /// </summary>
        public IEnumerable<FrameworkModel> GetAvailableFrameworks()
        {
            var retVals = new List<FrameworkModel>();
            foreach (MapperType value in Enum.GetValues(typeof(MapperType)))
            {
                retVals.Add(new FrameworkModel() { Name = value.ToString(), Id = (int)value });
            }

            return retVals;
        }
        /// <summary>
        /// <inheritdoc cref="IDataService.GetAvailableDatasetTypes"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DatasetTypeModel> GetAvailableDatasetTypes()
        {
            var retVals = new List<DatasetTypeModel>();
            foreach (DatasetType value in Enum.GetValues(typeof(DatasetType)))
            {
                if (value == DatasetType.Other)
                {
                    continue;
                }
                retVals.Add(new DatasetTypeModel() { Name = value.ToString(), Id = (int)value });
            }

            return retVals;
        }

        /// <summary>
        /// <inheritdoc cref="IDataService.GetRunLog"/>
        /// </summary>
        public List<string> GetRunLog()
        {
            string dir = DirectoryHelper.GetUserDocumentsFolder();
            var logFile = new FileInfo(Path.Combine(dir, Settings.RunLogFileName));

            return logFile.Exists ? File.ReadAllLines(logFile.FullName).ToList() : new();
        }
        /// <summary>
        /// <inheritdoc cref="IDataService.GetRunDataForChart"/>
        /// </summary>
        public ChartDataModel GetRunDataForChart(string runId)
        {
            string dir = DirectoryHelper.GetUserDocumentsFolder();

            if (IsComplete(dir, runId) == false)
                return default(ChartDataModel);

            ChartDataModel model = ReadRunDataFromFiles(dir, runId);

            return model;
        }

        /// <summary>
        /// Validates if all the files needed are present in the run folder
        /// </summary>
        /// <param name="dir">Directory that stores run history</param>
        /// <param name="runId">Identifier of the run to check</param>
        /// <returns>True if directory contains all the necessary files</returns>
        private bool IsComplete(string dir, string runId)
        {
            var runDir = Path.Combine(dir, runId);
            
            if (Directory.Exists(runDir) == false)
                return false;

            var files = Directory.GetFiles(runDir, "*", SearchOption.TopDirectoryOnly);
            
            return files.Length >= 2 
                   && files.Contains(Path.Combine(runDir, Settings.RunDataFileName)) 
                   && files.Contains(Path.Combine(runDir, $"{runId}.csv"));
        }
        /// <summary>
        /// Reads the data to the chart from files
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="runId"></param>
        /// <returns></returns>
        private ChartDataModel ReadRunDataFromFiles(string dir, string runId)
        {
            var runDir = Path.Combine(dir, runId);
            var runDataFile = Path.Combine(runDir, Settings.RunDataFileName);
            var resourceUsageFile = Path.Combine(runDir, $"{runId}.csv");

            var model = new ChartDataModel()
                .ParseRunDataFile(runDataFile)
                .ParseResourceUsage(resourceUsageFile)
                .ReadHardwareSpecifics()
                .CalculateUsageAverages();

            return model;
        }
    }
}