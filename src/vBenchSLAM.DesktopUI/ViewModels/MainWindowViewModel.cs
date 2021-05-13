using System;
using System.Reactive.Linq;
using ReactiveUI;
using vBenchSLAM.DesktopUI.Models;
using vBenchSLAM.DesktopUI.Services;
using vBenchSLAM.DesktopUI.ViewModels.Base;

namespace vBenchSLAM.DesktopUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _content;

        public ViewModelBase Content
        {
            get => _content;
            private set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        public MainWindowViewModel(IDataService dataService)
        {
            Content = new StartViewModel(dataService);
        }
    }
}