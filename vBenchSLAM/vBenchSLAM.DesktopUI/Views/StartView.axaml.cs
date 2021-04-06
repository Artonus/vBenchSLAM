using System;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;
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

        protected async void OpenFileButtonOnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn is null)
                return;

            var dialog = new OpenFolderDialog();

            var result = await dialog.ShowAsync((Window) Parent);

            var vm = GetViewModel();
            if (result.Any() && vm is not null)
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

        private async void BtnStartOnClick(object sender, RoutedEventArgs e)
        {
            var vm = GetViewModel();
            if (vm.HasErrors)
            {
                await MessageBox.Show((Window) Parent, "Please select all required fields", "Invalid arguments",
                    MessageBoxButtons.Ok);
                return;
            }

            try
            {
                var observer = new AnonymousObserver<Unit>(unit => { });
                vm.StartFrameworkCommand.Execute().Subscribe(observer);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Invalid dataset arguments");
                await MessageBox.Show((Window) Parent, ex.Message, "Invalid dataset arguments", MessageBoxButtons.Ok);
            }
        }

        private void ShowCurrentRunStats(object sender, RoutedEventArgs e)
        {
            var chart = new ChartWindow
            {
                DataContext = new ChartWindowViewModel(GetViewModel().DataService)
            };
            
            chart.Show((Window)this.Parent);
        }

        private void ShowRunStats(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}