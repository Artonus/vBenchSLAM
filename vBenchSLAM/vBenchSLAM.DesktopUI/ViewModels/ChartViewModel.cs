using vBenchSLAM.DesktopUI.Services;
using vBenchSLAM.DesktopUI.ViewModels.Base;

namespace vBenchSLAM.DesktopUI.ViewModels
{
    public class ChartViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        public ChartViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }
    }
}