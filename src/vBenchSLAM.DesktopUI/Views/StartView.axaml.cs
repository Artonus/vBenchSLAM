using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Serilog;
using vBenchSLAM.Addins;
using vBenchSLAM.DesktopUI.ViewModels;
using vBenchSLAM.DesktopUI.Windows;

namespace vBenchSLAM.DesktopUI.Views
{
    public class StartView : UserControl
    {
        public StartView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        /// <summary>
        /// Event handler. Opens the select folder dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected async void OpenFileButtonOnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn is null)
                return;

            var dialog = new OpenFolderDialog();

            var result = await dialog.ShowAsync((Window)Parent);

            var vm = GetViewModel();
            if (result is not null && result.Any())
            {
                if (btn.Name == nameof(StartViewModel.DatasetPath))
                    vm.DatasetPath = result;
                else if (btn.Name == nameof(StartViewModel.OutputPath))
                    vm.OutputPath = result;
            }
        }


        private StartViewModel GetViewModel()
        {
            return DataContext as StartViewModel;
        }
        /// <summary>
        /// Event handler. Starts the algorithm run
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnStartOnClick(object sender, RoutedEventArgs e)
        {
            var vm = GetViewModel();
            if (vm.HasErrors)
            {
                await MessageBox.Show((Window)Parent, "Please select all required fields", "Invalid arguments",
                    MessageBoxButtons.Ok);
                return;
            }

            try
            {
                await vm.StartFrameworkBenchmark();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Invalid dataset arguments");
                await MessageBox.Show((Window)Parent, ex.Message, "Invalid dataset arguments", MessageBoxButtons.Ok);
            }
        }
        /// <summary>
        /// Event handler. Shows all available windows with run results 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowCurrentRunStats(object sender, RoutedEventArgs e)
        {
            OpenChartWindow();
        }
        /// <summary>
        /// Event handler. Shows all available windows with run results 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowRunStats(object sender, RoutedEventArgs e)
        {
            OpenChartWindow(false);
        }
        /// <summary>
        /// Opens the vBenchSLAM result window
        /// </summary>
        /// <param name="openOnlyLatest"></param>
        private async void OpenChartWindow(bool openOnlyLatest = true)
        {
            var dataService = GetViewModel().DataService;
            var runs = dataService.GetRunLog();
            if (runs.Any() == false)
            {
                await MessageBox.Show((Window)Parent, "Couldn't find any runs. Make sure that you run the algorithm first.", "No benchmark data found", MessageBoxButtons.Ok);
#if DEBUG
                // when in the debug mode open an empty window
                var chart = new ChartWindow
                {
                    DataContext = new ChartWindowViewModel(GetViewModel().DataService, string.Empty)
                };

                chart.Show((Window)this.Parent);
#endif
                return;
            }

            var runsToOpen = openOnlyLatest ? new List<string>(new[] { runs.Last() }) : runs;

            foreach (var runId in runsToOpen)
            {
                try
                {
                    var chart = new ChartWindow
                    {
                        DataContext = new ChartWindowViewModel(GetViewModel().DataService, runId)
                    };

                    chart.Show((Window)this.Parent);
                }
                catch (Exception)
                {
                    Log.Error("Could not open the chart window");
                }

            }
        }
    }
}