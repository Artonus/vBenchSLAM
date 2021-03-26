using System.Reactive;
using ReactiveUI;
using vBenchSLAM.DesktopUI.Models;
using vBenchSLAM.DesktopUI.ViewModels.Base;

namespace vBenchSLAM.DesktopUI.ViewModels
{
    public class AddItemViewModel : ViewModelBase
    {
        private string _description;
        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        public ReactiveCommand<Unit, TodoItem> OkCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        public AddItemViewModel()
        {
            var okEnabled = this.WhenAnyValue(
                x => x.Description,
                x => string.IsNullOrWhiteSpace(x) == false);

            OkCommand = ReactiveCommand.Create(
                () => new TodoItem {Description = Description}, okEnabled);
            CancelCommand = ReactiveCommand.Create(() => { });
        }
    }
}