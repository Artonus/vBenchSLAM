using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using vBenchSLAM.DesktopUI.ViewModels;

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

        protected async void ButtonOnClick(object sender, RoutedEventArgs e)
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
    }
}