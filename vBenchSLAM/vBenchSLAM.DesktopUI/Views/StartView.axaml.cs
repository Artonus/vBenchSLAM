using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;
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
            
            var dialog = new OpenFileDialog()
            {
                AllowMultiple = false
            };
            
            var result = await dialog.ShowAsync((Window)Parent);
            
            var vm = GetViewModel();
            if (result.Any() && vm is not null)
            {
                if (btn.Name == nameof(StartViewModel.DatasetPath))
                    vm.DatasetPath = result.First();
                else if (btn.Name == nameof(StartViewModel.OutputPath))
                    vm.OutputPath = result.First();
            }
        }
        

        private StartViewModel GetViewModel()
        {
            return DataContext as StartViewModel;
        }

        private async void BtnStartOnClick(object? sender, RoutedEventArgs e)
        {
            var vm = GetViewModel();
            if (vm.HasErrors)
            {
                await MessageBox.Show((Window)Parent, "Please select all required fields", "Invalid arguments",
                    MessageBoxButtons.Ok);
                return;
            }

            var observer = new AnonymousObserver<Unit>(unit => { });
            vm.StartFrameworkCommand.Execute().Subscribe(observer);
        }
    }
}