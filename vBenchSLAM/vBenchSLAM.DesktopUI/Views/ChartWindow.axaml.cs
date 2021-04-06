using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using vBenchSLAM.DesktopUI.Services;

namespace vBenchSLAM.DesktopUI.Views
{
    public class ChartWindow : Window
    {

        public ChartWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public ChartWindow(IDataService dataService) : this()
        {
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
