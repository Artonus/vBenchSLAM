using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace vBenchSLAM.UI
{
    public class MainForm : Window
    {
        public MainForm()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

        }
    }
}
