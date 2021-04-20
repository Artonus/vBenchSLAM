using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Renderable;
using vBenchSLAM.DesktopUI.ViewModels;
using Color = System.Drawing.Color;

namespace vBenchSLAM.DesktopUI.Views
{
    enum ChartType
    {
        Ram,
        Cpu
    }
    public class ChartView : UserControl
    {
        private bool _hasLoadedChartData;
        public ChartView()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnDataContextChanged(object sender, EventArgs e)
        {
            if (_hasLoadedChartData == false)
            {
                LoadData();
                _hasLoadedChartData = true;
            }
        }

        private ChartViewModel GetViewModel()
        {
            return DataContext as ChartViewModel;
        }

        private void LoadData()
        {
            var dataModel = GetViewModel().DataModel;

            if (dataModel is null)
                return;

            AvaPlot ramUsagePlot = this.Find<AvaPlot>("RamUsagePlot");
            AvaPlot cpuUsagePlot = this.Find<AvaPlot>("CpuUsagePlot");
            double[] axisXdata = Enumerable.Range(0, dataModel.ResourceUsages.Count).Select(e => (double)e).ToArray();

            double[] ramAxisYdata = dataModel.ResourceUsages.Select(u => Convert.ToDouble(u.RamUsage)).ToArray();
            double[] maxRamAxisYdata = dataModel.ResourceUsages.Select(u => Convert.ToDouble(u.MaxRamAvailable)).ToArray();
            double[] cpuAxisYdata = dataModel.ResourceUsages.Select(u => Convert.ToDouble(u.ProcUsage)).ToArray();
            double[] ramPercentAxisYdata = dataModel.ResourceUsages.Select(u => Convert.ToDouble(u.RamPercentUsage)).ToArray();
            ramUsagePlot.Plot.AddScatter(axisXdata, ramAxisYdata, Color.Blue, 2F, label: "RAM usage");
            ramUsagePlot.Plot.AddScatter(axisXdata, maxRamAxisYdata, Color.Red, 2F, label: "Max RAM avaliable");

            // TODO: add secondary axis to scale the RAM usage
            var scatter = cpuUsagePlot.Plot.AddScatter(axisXdata, cpuAxisYdata, Color.Blue, 2F, label: "% CPU usage");
            //scatter.YAxisIndex = 0;

            var ramScatter = cpuUsagePlot.Plot.AddScatter(axisXdata, ramPercentAxisYdata, Color.Red, 2F, label: "% RAM usage");
            // var secAxis = ramUsagePlot.Plot.AddAxis(Edge.Right, axisIndex: 2);
            // ramScatter.YAxisIndex = 2;
            // secAxis.Color(ramScatter.Color);
            // secAxis.Label("% RAM usage");

            // set labels
            StylePlot(ramUsagePlot.Plot, ChartType.Ram);
            StylePlot(cpuUsagePlot.Plot, ChartType.Cpu);


            GetViewModel().PrepareRecommendations(dataModel);
        }

        private void StylePlot(Plot plt, ChartType chartType)
        {
            plt.XLabel("Ticks");
            if (chartType == ChartType.Cpu)
            {
                plt.YLabel("% CPU usage");
            }

            if (chartType == ChartType.Ram)
            {
                plt.YLabel("RAM memory");
            }

        }
    }
}
