using System;
using System.IO;
using System.Linq;
using vBenchSLAM.Core.MapParser.Models;
using vBenchSLAM.Core.Model;
using vBenchSLAM.DesktopUI.Models;

namespace vBenchSLAM.DesktopUI.ExtensionMethods
{
    public static class ChartDataModelExtensionMethods
    {
        public static ChartDataModel ParseRunDataFile(this ChartDataModel model, string runDataFilePath)
        {
            var fileContent = File.ReadAllLines(runDataFilePath);

            for (int i = 0; i < fileContent.Length; i++) // we skip the first line as it is the header line
            {
                if (i == 0 || i == 2) // skip the header lines
                    continue;
                var spitted = fileContent[i].Split(';', StringSplitOptions.RemoveEmptyEntries);
                
                if (i == 1)
                {
                    model.Started = DateTime.Parse(spitted[0]);
                    model.Finished = DateTime.Parse(spitted[1]);
                    model.Framework = spitted[2];
                }
                if (i == 3)
                {
                    var data = MapData.FromCsvLiteral(fileContent[i]);
                    model.Keyframes = data.Keyframes;
                    model.Keypoints = data.Keypoints;
                    model.Landmarks = data.Landmarks;
                }
            }
            return model;
        }
        public static ChartDataModel ParseResourceUsage(this ChartDataModel model, string resourceUsageFile)
        {
            var lines = File.ReadAllLines(resourceUsageFile);

            foreach (var line in lines)
            {
                var parsedModel = ResourceUsage.FromCsvLiteral(line);
                model.ResourceUsages.Add(parsedModel);
            }

            return model;
        }

        public static ChartDataModel ReadHardwareSpecifics(this ChartDataModel model)
        {
            model.Cores = Environment.ProcessorCount;
            model.Ram = model.ResourceUsages.First().MaxRamAvailable;
            //TODO: get cpu name
            model.Cpu = string.Empty;
            return model;
        }

        public static ChartDataModel CalculateUsageAverages(this ChartDataModel model)
        {
            model.AvgCpuUsage = model.ResourceUsages.Average(r => r.ProcUsage);
            model.AvgRamUsage = model.ResourceUsages.Average(r => r.RamPercentUsage);
            return model;
        }
    }
}
