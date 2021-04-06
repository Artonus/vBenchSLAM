using System;
using ReactiveUI;
using vBenchSLAM.Addins.ExtensionMethods;
using vBenchSLAM.Core.Enums;
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
        public ChartViewModel(IDataService dataService)
        {
            _dataService = dataService;
            DataModel = new ChartDataModel()
            {
                Keyframes = 23,
                Keypoints = 48,
                Landmarks = 10,
                AvgCpuUsage = 95.3M,
                AvgRamUsage = 45.7M,
                Cores = 6,
                Cpu = "Amd Ryzen 3600",
                Finished = DateTime.Now,
                Framework = MapperTypeEnum.OpenVslam.GetStringValue(),
                Ram = 123456,
                Started = DateTime.Now.AddMinutes(-3)
            };
        }
    }
}