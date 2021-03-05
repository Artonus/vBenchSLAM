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
        private readonly IDataService _dataService;
        private ViewModelBase _content;
        public TodoListViewModel List { get; }

        public ViewModelBase Content
        {
            get => _content;
            private set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        public MainWindowViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Content = new StartViewModel(dataService);
        }

        public void AddItem()
        {
            var vm = new AddItemViewModel();
            Observable.Merge(
                    vm.OkCommand,
                    vm.CancelCommand.Select(_ => (TodoItem) null))
                .Take(1)
                .Subscribe(model =>
                {
                    if (model is not null)
                        List.Items.Add(model);
                    
                    Content = List;
                });
            Content = vm;
        }
    }
}