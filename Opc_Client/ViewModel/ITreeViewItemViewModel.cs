using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Opc_Client.ViewModel
{
    public interface ITreeViewItemViewModel
    {
        ObservableCollection<ITreeViewItemViewModel> Children { get; set; }
        bool HasDummyChild { get; }
        bool IsExpanded { get; set; }
        bool IsSelected { get; set; }
    }
}