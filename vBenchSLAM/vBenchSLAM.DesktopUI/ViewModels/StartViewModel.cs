using System.Collections.ObjectModel;
using ReactiveUI;
using vBenchSLAM.DesktopUI.Models;
using vBenchSLAM.DesktopUI.Services;
using vBenchSLAM.DesktopUI.ViewModels.Base;

namespace vBenchSLAM.DesktopUI.ViewModels
{
    public class StartViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private string _datasetInformation;
        private string _datasetPath;
        private string _outputPath;
        public ObservableCollection<FrameworkModel> FrameworkList { get; }

        public string DatasetInformation
        {
            get => _datasetInformation;
            set => this.RaiseAndSetIfChanged(ref _datasetInformation, value);
        }

        public string DatasetPath
        {
            get => _datasetPath;
            set => this.RaiseAndSetIfChanged(ref _datasetPath, value);
        }

        public string OutputPath
        {
            get => _outputPath;
            set => this.RaiseAndSetIfChanged(ref _outputPath, value);
        }
        public StartViewModel(IDataService dataService)
        {
            _dataService = dataService;
            var availableFrameworks = dataService.GetAvailableFrameworks();
            FrameworkList = new ObservableCollection<FrameworkModel>(availableFrameworks);
        }
        
        
    }
}