using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using vBenchSLAM.Addins;
using vBenchSLAM.Addins.ExtensionMethods;
using vBenchSLAM.Core;
using vBenchSLAM.Core.Model;
using vBenchSLAM.DesktopUI.Models;
using vBenchSLAM.DesktopUI.Services;
using vBenchSLAM.DesktopUI.ViewModels.Base;

namespace vBenchSLAM.DesktopUI.ViewModels
{
    public class StartViewModel : ViewModelBase
    {
        /// <summary>
        /// Dataset Service instance
        /// </summary>
        public IDataService DataService { get; }

        public ObservableCollection<FrameworkModel> FrameworkList { get; }

        private FrameworkModel _selectedFramework;
        /// <summary>
        /// Framework selected in the UI
        /// </summary>
        public FrameworkModel SelectedFramework
        {
            get => _selectedFramework;
            set => this.RaiseAndSetIfChanged(ref _selectedFramework, value);
        }
        /// <summary>
        /// Collection of available dataset types
        /// </summary>
        public ObservableCollection<DatasetTypeModel> DatasetTypeList { get; }

        private DatasetTypeModel _selectedDatasetType;
        /// <summary>
        /// Dataset type selected in the UI
        /// </summary>
        public DatasetTypeModel SelectedDatasetType
        {
            get => _selectedDatasetType;
            set => this.RaiseAndSetIfChanged(ref _selectedDatasetType, value);
        }
        
        private string _datasetPath;
        /// <summary>
        /// Dataset path selected in the UI
        /// </summary>
        public string DatasetPath
        {
            get => _datasetPath;
            set => this.RaiseAndSetIfChanged(ref _datasetPath, value);
        }

        private string _outputPath;
        /// <summary>
        /// Output path selected in the UI
        /// </summary>
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
            // set up the starting paths in the DEV build
            DatasetPath = "/home/bartek/Works/vBenchSLAM/Samples/Kitty";
            OutputPath = "/home/bartek/Works/vBenchSLAM/Samples/Output";
            SelectedFramework = availableFrameworks.First(f => f.Id == (int) MapperType.OpenVslam);
            SelectedDatasetType = availableDatasetTypes.First(f => f.Id == (int) DatasetType.Kitty);
#endif
            PrepareValidationConstraints();
        }
        /// <summary>
        /// Prepares the validation for the control
        /// </summary>
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
        /// <summary>
        /// Starts the benchmarking of the selected framework
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Gets the selected dataset type
        /// </summary>
        /// <returns></returns>
        private DatasetType GetSelectedDatasetType()
        {
            return (DatasetType) SelectedDatasetType.Id;
        }
        /// <summary>
        /// Gets the selected mapper type
        /// </summary>
        /// <returns></returns>
        private MapperType GetSelectedMapperType()
        {
            return (MapperType) SelectedFramework.Id;
        }
    }
}