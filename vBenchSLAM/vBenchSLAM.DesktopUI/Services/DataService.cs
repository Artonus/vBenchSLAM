using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using vBenchSLAM.Addins;
using vBenchSLAM.Core;
using vBenchSLAM.Core.Enums;
using vBenchSLAM.DesktopUI.Models;

namespace vBenchSLAM.DesktopUI.Services
{
    public class DataService : IDataService
    {
        /// <summary>
        /// <inheritdoc cref="IDataService.GetAvailableFrameworks"/>
        /// </summary>
        public IEnumerable<FrameworkModel> GetAvailableFrameworks()
        {
            var retVals = new List<FrameworkModel>();
            foreach (MapperTypeEnum value in Enum.GetValues(typeof(MapperTypeEnum)))
            {
                retVals.Add(new FrameworkModel(){Name = value.ToString(), Id = (int)value});
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

            return logFile.Exists ? File.ReadAllLines(logFile.FullName).ToList() : new ();
        }

        public ChartDataModel GetRunDataForChart(string run)
        {
            string dir = DirectoryHelper.GetUserDocumentsFolder();
            //TODO
            throw new NotImplementedException();
        }
    }
}