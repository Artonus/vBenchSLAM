using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using Serilog;
using vBenchSLAM.Addins.ExtensionMethods;
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
        public IDataService DataService { get; }
        private FrameworkModel _selectedFramework;
        private DatasetTypeModel _selectedDatasetType;
        private string _datasetPath;
        private string _outputPath;

        //public ValidationContext ValidationContext { get; } 
        public ObservableCollection<FrameworkModel> FrameworkList { get; }
        
        public FrameworkModel SelectedFramework
        {
            get => _selectedFramework;
            set => this.RaiseAndSetIfChanged(ref _selectedFramework, value);
        }
        public ObservableCollection<DatasetTypeModel> DatasetTypeList { get; }
        public DatasetTypeModel SelectedDatasetType
        {
            get => _selectedDatasetType;
            set => this.RaiseAndSetIfChanged(ref _selectedDatasetType, value);
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
            DataService = dataService;
            
            var availableFrameworks = dataService.GetAvailableFrameworks();
            FrameworkList = new ObservableCollection<FrameworkModel>(availableFrameworks);

            var availableDatasetTypes = dataService.GetAvailableDatasetTypes();
            DatasetTypeList = new ObservableCollection<DatasetTypeModel>(availableDatasetTypes);

#if DEBUG
            DatasetPath = "/home/bartek/Works/vBenchSLAM/Samples/Kitty";
            OutputPath = "/home/bartek/Works/vBenchSLAM/Samples/Output";
            SelectedFramework = availableFrameworks.First(f => f.Id == (int) MapperType.OrbSlam);
            SelectedDatasetType = availableDatasetTypes.First(f => f.Id == (int) DatasetType.Kitty);
#endif
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
            this.ValidationRule(vm => vm.SelectedDatasetType,
                f => f is not null,
                "Please select desired dataset type");
        }

        public async Task StartFrameworkBenchmark()
        {
            if (HasErrors)
            {
                return;
            }
            
            var mapperType = GetSelectedMapperType();
            var datasetType = GetSelectedDatasetType();

            var param = new RunnerParameters(mapperType, datasetType, OutputPath, DatasetPath);
            RunnerResult result = null;
            using (IRunner runner = new Runner(param))
            {
                result = await runner.Run();
            }

            if (result.IsSuccess)
            {
                Log.Information($"Successfully completed the benchmark of the {mapperType.GetStringValue()} algorithm");
            }
            else
            {
                throw new Exception("The algorithm did not run properly!", result.Exception);
            }
        }

        private DatasetType GetSelectedDatasetType()
        {
            return (DatasetType) SelectedDatasetType.Id;
        }

        private MapperType GetSelectedMapperType()
        {
            return (MapperType) SelectedFramework.Id;
        }
    }
}