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
        }

        public ChartViewModel(IDataService dataService, string runId) : this(dataService)
        {
            DataModel = dataService.GetRunDataForChart(runId);
        }
    }
}