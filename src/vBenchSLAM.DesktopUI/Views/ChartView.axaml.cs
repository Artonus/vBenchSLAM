using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Renderable;
using System;
using System.Linq;
using vBenchSLAM.Addins;
using vBenchSLAM.Addins.Models;
using vBenchSLAM.DesktopUI.ViewModels;
using Color = System.Drawing.Color;
using Size = vBenchSLAM.Addins.Size;

namespace vBenchSLAM.DesktopUI.Views
{
    enum ChartType
    {
        Ram,
        Cpu
    }
    public class ChartView : UserControl
    {

        private readonly Size _displaySize = Size.GB;
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
        /// <summary>
        /// DataContext changed event handler, responsible for loading the data into the chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataContextChanged(object sender, EventArgs e)
        {
            if (_hasLoadedChartData == false)
            {
                LoadData();
                _hasLoadedChartData = true;

                HideEmptyRecommendationLabels();
            }
        }
        /// <summary>
        /// Hides the empty recommendation labels
        /// </summary>
        private void HideEmptyRecommendationLabels()
        {
            var vm = GetViewModel();
            if (string.IsNullOrEmpty(vm.Fatal))
            {
                var fatalLabel = this.Find<Label>("FatalLabel");
                fatalLabel.IsVisible = false;
            }
            
            if (string.IsNullOrEmpty(vm.Improvements))
            {
                var improvementsLabel = this.Find<Label>("ImprovementsLabel");
                improvementsLabel.IsVisible = false;
            }
            
            if (string.IsNullOrEmpty(vm.AlreadyGood))
            {
                var alreadyGoodLabel = this.Find<Label>("AlreadyGoodLabel");
                alreadyGoodLabel.IsVisible = false;
            }
        }
        /// <summary>
        /// Returns the view model associated with the current view
        /// </summary>
        /// <returns></returns>
        private ChartViewModel GetViewModel()
        {
            return DataContext as ChartViewModel;
        }
        /// <summary>
        /// Loads the data into the charts
        /// </summary>
        private void LoadData()
        {
            var vm = GetViewModel();
            var dataModel = vm.DataModel;

            if (dataModel is null)
                return;
            
            double[] axisXData = Enumerable.Range(0, dataModel.ResourceUsages.Count).Select(e => (double)e).ToArray();
            PrepareRamChart(dataModel, axisXData);
            
            PrepareCpuChart(dataModel, axisXData);

            vm.PrepareRecommendations(dataModel);
        }
        /// <summary>
        /// Prepares the chart with the CPU and GPU usage
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="axisXData"></param>
        private void PrepareCpuChart(ChartDataModel dataModel, double[] axisXData)
        {
            AvaPlot cpuUsagePlot = this.Find<AvaPlot>("CpuUsagePlot");
            cpuUsagePlot.Plot.Legend(true, Alignment.UpperLeft);

            double[] cpuAxisYData = dataModel.ResourceUsages.Select(u => Convert.ToDouble(u.ProcUsage)).ToArray();            
            double[] gpuPercentAxisYData = dataModel.ResourceUsages.Select(u => Convert.ToDouble(u.GPUUsage)).ToArray();
            // cpu series
            var cpuScatter = cpuUsagePlot.Plot.AddScatter(axisXData, cpuAxisYData, Color.Blue, 2F, label: "% CPU usage");
            cpuScatter.YAxisIndex = cpuScatter.XAxisIndex = 0;
            // gpu series
            var gpuScatter = cpuUsagePlot.Plot.AddScatter(axisXData, gpuPercentAxisYData, Color.Green, 2F, label: "% GPU usage");
            // secondary axis
            var secAxis = cpuUsagePlot.Plot.AddAxis(Edge.Right, axisIndex: 2);
            //secAxis.Color(ramScatter.Color);
            secAxis.Label("% GPU usage");
            // set series to secondary axis
            gpuScatter.YAxisIndex = 2;
            // set labels
            StylePlot(cpuUsagePlot.Plot, ChartType.Cpu);
        }
        /// <summary>
        /// Prepares the chart with the RAM usage
        /// </summary>
        /// <param name="data"></param>
        /// <param name="axisXData"></param>
        private void PrepareRamChart(ChartDataModel data, double[] axisXData)
        {
            AvaPlot ramUsagePlot = this.Find<AvaPlot>("RamUsagePlot");
            // add series    
            double[] ramAxisYData = data.ResourceUsages.Select(u => Convert.ToDouble(SizeHelper.SizeValue(u.RamUsage, _displaySize))).ToArray();
            double[] maxRamAxisYData = data.ResourceUsages.Select(u => Convert.ToDouble(SizeHelper.SizeValue(u.MaxRamAvailable, _displaySize))).ToArray();
            double[] ramPercentAxisYData = data.ResourceUsages.Select(u => Convert.ToDouble(u.RamPercentUsage)).ToArray();
            ramUsagePlot.Plot.AddScatter(axisXData, ramAxisYData, Color.Blue, 2F, label: "RAM usage");
            ramUsagePlot.Plot.AddScatter(axisXData, maxRamAxisYData, Color.Red, 2F, label: "Max RAM avaliable");
            // % ram series
            var ramScatter = ramUsagePlot.Plot.AddScatter(axisXData, ramPercentAxisYData, Color.Green, 2F, label: "% RAM usage");
            // secondary axis
            var secAxis = ramUsagePlot.Plot.AddAxis(Edge.Right, axisIndex: 2);
            secAxis.Label("% RAM usage");
            // set series to secondary axis  
            ramScatter.YAxisIndex = 2;
            StylePlot(ramUsagePlot.Plot, ChartType.Ram);
            
            ramUsagePlot.Plot.Legend(true, Alignment.UpperLeft);
        }
        /// <summary>
        /// Sets the style of a plot
        /// </summary>
        /// <param name="plt"></param>
        /// <param name="chartType"></param>
        private void StylePlot(Plot plt, ChartType chartType)
        {
            plt.XLabel("Run duration in seconds");
            if (chartType == ChartType.Cpu)
            {
                plt.YLabel("% CPU usage");
            }

            if (chartType == ChartType.Ram)
            {
                plt.YLabel($"RAM memory in {_displaySize}");
            }

        }
    }
}
