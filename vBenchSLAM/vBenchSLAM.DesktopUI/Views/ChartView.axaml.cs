using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace vBenchSLAM.DesktopUI.Views
{
    public class ChartView : UserControl
    {
        public ChartView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
