using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ScottPlot.Avalonia;
using vBenchSLAM.DesktopUI.ViewModels;
using Color = System.Drawing.Color;

namespace vBenchSLAM.DesktopUI.Views
{
    public class ChartView : UserControl
    {
        public ChartView()
        {
            InitializeComponent();

            LoadDataToChart();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private ChartViewModel GetViewModel()
        {
            return DataContext as ChartViewModel;
        }
        private void LoadDataToChart()
        {
            var dataModel = GetViewModel().DataModel;
            AvaPlot ramUsagePlot = this.Find<AvaPlot>("RamUsagePlot");
            AvaPlot cpuUsagePlot = this.Find<AvaPlot>("CpuUsagePlot");
            double[] axisXdata = Enumerable.Range(0, dataModel.ResourceUsages.Count).Select(e=> (double)e).ToArray();

            double[] ramAxisYdata = dataModel.ResourceUsages.Select(u => Convert.ToDouble(u.RamUsage)).ToArray();
            double[] maxRamAxisYdata = dataModel.ResourceUsages.Select(u => Convert.ToDouble(u.MaxRamAvailable)).ToArray();
            double[] cpuAxisYdata = dataModel.ResourceUsages.Select(u => Convert.ToDouble(u.RamUsage)).ToArray();
            ramUsagePlot.Plot.AddScatter(axisXdata, ramAxisYdata, Color.Blue, 2F);
            ramUsagePlot.Plot.AddScatter(axisXdata, maxRamAxisYdata, Color.Red, 2F);
            cpuUsagePlot.Plot.AddScatter(axisXdata, cpuAxisYdata, Color.Blue, 2F);
        }
    }
}
