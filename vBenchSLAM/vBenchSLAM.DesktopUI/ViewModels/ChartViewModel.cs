using System;
using ReactiveUI;
using vBenchSLAM.Addins.ExtensionMethods;
using vBenchSLAM.Core.Enums;
using vBenchSLAM.Core.MapParser.Models;
using vBenchSLAM.DesktopUI.Models;
using vBenchSLAM.DesktopUI.Services;
using vBenchSLAM.DesktopUI.ViewModels.Base;

namespace vBenchSLAM.DesktopUI.ViewModels
{
    public class ChartViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        private ChartDataModel _data;
        public ChartDataModel DataModel
        {
            get => _data;
            set => this.RaiseAndSetIfChanged(ref _data, value);
        }
        private string _fatal;

        public string Fatal
        {
            get => _fatal;
            set => this.RaiseAndSetIfChanged(ref _fatal, value);
        }

        private string _improvements;

        public string Improvements
        {
            get => _improvements;
            set => this.RaiseAndSetIfChanged(ref _improvements, value);
        }
        
        private string _alreadyGood;

        public string AlreadyGood
        {
            get => _alreadyGood;
            set => this.RaiseAndSetIfChanged(ref _alreadyGood, value);
        }


        public ChartViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        public ChartViewModel(IDataService dataService, string runId) : this(dataService)
        {
            DataModel = dataService.GetRunDataForChart(runId);
        }

        public void PrepareRecommendations(ChartDataModel data)
        {
            ClearExistingRecommendations();

            PrepareCpuRecommendations(data);
            PrepareMemoryRecommendations(data);
        }

        private void ClearExistingRecommendations()
        {
            AlreadyGood = Improvements = Fatal = string.Empty;
        }

        private void PrepareCpuRecommendations(ChartDataModel data)
        {
            var cpuCount = Environment.ProcessorCount / 2;
            decimal maxCpuUsage = cpuCount * 100;
            if (data.AvgCpuUsage < maxCpuUsage * 0.5M)
            {
                AlreadyGood += $"Your machine has powerful enough processor and GPU. {Environment.NewLine}";
            }
            else if (data.AvgCpuUsage >= maxCpuUsage * 0.5M && data.AvgCpuUsage <= maxCpuUsage * 0.9M)
            {
                Improvements +=
                    $"Your machine has utilized {data.AvgCpuUsage}% of {maxCpuUsage} CPU available. You may want to consider using the more powerful CPU in the future or use the GPU. {Environment.NewLine}";
            }
            else if (data.AvgCpuUsage >= maxCpuUsage * 0.9M)
            {
                Fatal +=
                    $"Your machine has crossed the CPU utilization of {maxCpuUsage * 0.9M}% of CPU. Your machine may experience delays on the frames computation and loss in the quality of the map. {Environment.NewLine}";
            }
        }
        private void PrepareMemoryRecommendations(ChartDataModel data)
        {
            var maxMemory = data.Ram;
            
            if (data.AvgCpuUsage < 50)
            {
                AlreadyGood += $"Your machine has more RAM then needed to run the algorithm. {Environment.NewLine}";
            }
            else if (data.AvgCpuUsage is >= 50 and <= 90)
            {
                Improvements +=
                    $"Your machine has enough RAM to easily run the the algorithm. It is not needed to add more of it. {Environment.NewLine}";
            }
            else if (data.AvgCpuUsage >= 90)
            {
                Fatal +=
                    $"Your machine has crossed the RAM utilization of 90%. When running longer you may experience unwanted issues related to the swap memory utilization, slowing the responsiveness of the algorithm or even stopping the applications.{Environment.NewLine}";
            }
        }
        
    }
}