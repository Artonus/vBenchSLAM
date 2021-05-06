using System;
using Avalonia.Media;
using ReactiveUI;
using vBenchSLAM.Addins.ExtensionMethods;
using vBenchSLAM.Addins.Models;
using vBenchSLAM.DesktopUI.Models;
using vBenchSLAM.DesktopUI.Services;
using vBenchSLAM.DesktopUI.ViewModels.Base;

namespace vBenchSLAM.DesktopUI.ViewModels
{
    public class ChartViewModel : ViewModelBase
    {
        /// <summary>
        /// Data Service instance
        /// </summary>
        private readonly IDataService _dataService;

        private ChartDataModel _data;
        /// <summary>
        /// Chart data
        /// </summary>
        public ChartDataModel DataModel
        {
            get => _data;
            set => this.RaiseAndSetIfChanged(ref _data, value);
        }
        private string _fatal;
        /// <summary>
        /// Fatal recommendations to the system
        /// </summary>
        public string Fatal
        {
            get => _fatal;
            set => this.RaiseAndSetIfChanged(ref _fatal, value);
        }

        private IBrush _fatalColorBrush;
        /// <summary>
        /// Color of fatal recommendations to the system
        /// </summary>
        public IBrush FatalColorBrush
        {
            get => _fatalColorBrush;
            set => this.RaiseAndSetIfChanged(ref _fatalColorBrush, value);
        }

        private string _improvements;
        /// <summary>
        /// Improvements recommendations to the system
        /// </summary>
        public string Improvements
        {
            get => _improvements;
            set => this.RaiseAndSetIfChanged(ref _improvements, value);
        }
        private IBrush _improvementsColorBrush;
        /// <summary>
        /// Color of improvements recommendations to the system
        /// </summary>
        public IBrush ImprovementsColorBrush
        {
            get => _improvementsColorBrush;
            set => this.RaiseAndSetIfChanged(ref _improvementsColorBrush, value);
        }

        private string _alreadyGood;
        /// <summary>
        /// Already Good recommendations to the system
        /// </summary>
        public string AlreadyGood
        {
            get => _alreadyGood;
            set => this.RaiseAndSetIfChanged(ref _alreadyGood, value);
        }
        
        private IBrush _alreadyGoodColorBrush;
        /// <summary>
        /// Color of already good recommendations to the system
        /// </summary>
        public IBrush AlreadyGoodColorBrush
        {
            get => _alreadyGoodColorBrush;
            set => this.RaiseAndSetIfChanged(ref _alreadyGoodColorBrush, value);
        }

        public ChartViewModel(IDataService dataService)
        {
            _dataService = dataService;
            FatalColorBrush = Brushes.Red;
            ImprovementsColorBrush = Brushes.Orange;
            AlreadyGoodColorBrush = Brushes.Green;
        }

        public ChartViewModel(IDataService dataService, string runId) : this(dataService)
        {
            DataModel = dataService.GetRunDataForChart(runId);
        }
        /// <summary>
        /// Prepares the recommendations for the current setup based on the gathered data
        /// </summary>
        /// <param name="data"></param>
        public void PrepareRecommendations(ChartDataModel data)
        {
            ClearExistingRecommendations();

            PrepareCpuRecommendations(data);
            PrepareRamRecommendations(data);
            PrepareGpuRecommendations(data);
        }
        /// <summary>
        /// Clears the recommendations that has been calculated for the current run
        /// </summary>
        private void ClearExistingRecommendations()
        {
            AlreadyGood = Improvements = Fatal = string.Empty;
        }
        /// <summary>
        /// Prepares the CPU recommendations to the user based on the CPU usage
        /// </summary>
        /// <param name="data"></param>
        private void PrepareCpuRecommendations(ChartDataModel data)
        {
            var cpuCount = data.Cores / 2;
            decimal maxCpuUsage = cpuCount * 100;
            if (data.AvgCpuUsage < maxCpuUsage * 0.5M)
            {
                AlreadyGood += $"Your machine has powerful enough processor and GPU. {Environment.NewLine}";
            }
            else if (data.AvgCpuUsage >= maxCpuUsage * 0.5M && data.AvgCpuUsage <= maxCpuUsage * 0.95M)
            {
                Improvements +=
                    $"Your machine has utilized {data.AvgCpuUsage} of maximum {maxCpuUsage}% CPU available. You may want to consider using the more powerful CPU in the future or use the algorithm that can utilise the GPU. {Environment.NewLine}";
            }
            else if (data.AvgCpuUsage >= maxCpuUsage * 0.95M)
            {
                Fatal +=
                    $"Your machine has crossed the average CPU utilization of {maxCpuUsage * 0.9M}% of CPU. Your machine may experience delays on the frames computation and loss in the quality of the map. {Environment.NewLine}";
            }
        }
        /// <summary>
        /// Prepares the RAM recommendations to the user based on the RAM usage
        /// </summary>
        /// <param name="data"></param>
        private void PrepareRamRecommendations(ChartDataModel data)
        {
            switch (data.AvgRamUsage)
            {
                case < 50:
                    AlreadyGood += $"Your machine has more RAM then needed to run the algorithm. {Environment.NewLine}";
                    break;
                case >= 50 and < 90:
                    Improvements +=
                        $"Your machine has enough RAM to easily run the the algorithm. It is not needed to add more of it. {Environment.NewLine}";
                    break;
                case >= 90:
                    Fatal +=
                        $"Your machine has crossed the RAM utilization of 90%. When running longer you may experience unwanted issues related to the swap memory utilization, slowing the responsiveness of the algorithm or even stopping the applications.{Environment.NewLine}";
                    break;
            }
        }
        /// <summary>
        /// Prepares the GPU recommendations to the user based on the GPU usage
        /// </summary>
        /// <param name="data"></param>
        private void PrepareGpuRecommendations(ChartDataModel data)
        {
            switch (data.AvgGpuUsage)
            {
                case 0m:
                    Improvements += $"Your machine can improve the efficiency if you supply it with the GPU. {Environment.NewLine}";
                    break;
                case < 50:
                    AlreadyGood += $"Your machine has more powerful GPU then needed to run the algorithm. {Environment.NewLine}";
                    break;
                case >= 50 and < 95:
                    Improvements +=
                        $"Your machine has powerful enough GPU to easily run the the algorithm. It is not needed to add more of it. {Environment.NewLine}";
                    break;
                case >= 95:
                    Fatal +=
                        $"Your machine has crossed the GPU utilization of 90%. When running you may experience slowing the responsiveness of the algorithm.{Environment.NewLine}";
                    break;
            }
        }
    }
}