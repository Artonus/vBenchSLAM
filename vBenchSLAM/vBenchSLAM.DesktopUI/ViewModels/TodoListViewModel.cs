using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using vBenchSLAM.DesktopUI.Models;
using vBenchSLAM.DesktopUI.ViewModels.Base;

namespace vBenchSLAM.DesktopUI.ViewModels
{
    public class TodoListViewModel : ViewModelBase
    {
        public ObservableCollection<TodoItem> Items { get; }
        
        public TodoListViewModel(IEnumerable<TodoItem> items)
        {
            Items = new ObservableCollection<TodoItem>(items);
        }
    }
}