using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using vBenchSLAM.Core;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.Enums;
using vBenchSLAM.Core.Model;
using vBenchSLAM.DesktopUI.Models;
using vBenchSLAM.DesktopUI.Services;
using vBenchSLAM.DesktopUI.ViewModels.Base;

namespace vBenchSLAM.DesktopUI.ViewModels
{
    public class StartViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private FrameworkModel _selectedFramework;
        private string _datasetInformation;
        private string _datasetPath;
        private string _outputPath;

        //public ValidationContext ValidationContext { get; } 
        public ObservableCollection<FrameworkModel> FrameworkList { get; }
        public FrameworkModel SelectedFramework
        {
            get => _selectedFramework;
            set => this.RaiseAndSetIfChanged(ref _selectedFramework, value);
        }

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

        public ReactiveCommand<Unit, Unit> StartFrameworkCommand { get; }

        public StartViewModel(IDataService dataService)
        {
            _dataService = dataService;
            
            var availableFrameworks = dataService.GetAvailableFrameworks();
            FrameworkList = new ObservableCollection<FrameworkModel>(availableFrameworks);

            StartFrameworkCommand = ReactiveCommand.Create(StartFrameworkCommandHandler);

            PrepareValidationConstraints();
        }

        private void PrepareValidationConstraints()
        {
            //dataset path
            this.ValidationRule(vm => vm.DatasetPath,
                path => string.IsNullOrWhiteSpace(path) == false, //&& Directory.Exists(path) == false,
                "Please select appropriate dataset directory");
            this.ValidationRule(vm => vm.OutputPath,
                path => string.IsNullOrWhiteSpace(path) == false, //&& Directory.Exists(path) == false,
                "Please select appropriate output directory");
            this.ValidationRule(vm => vm.SelectedFramework,
                f => f is not null,
                "Please select desired framework");
        }

        private void StartFrameworkCommandHandler()
        {
            if (HasErrors)
            {
                return;
            }
            
            var mapperType = GetSelectedMapperType();

            var param = new RunnerParameters(mapperType, DatasetPath, OutputPath);

            using (var runner = new Runner(param))
            {
                runner.Run();
            }
        }

        private MapperTypeEnum GetSelectedMapperType()
        {
            return (MapperTypeEnum)SelectedFramework.Id; 
        }
    }
}