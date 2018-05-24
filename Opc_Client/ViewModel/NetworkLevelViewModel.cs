using GalaSoft.MvvmLight;
using Opc_Client.Helpers;
using OpcClientLibrary;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Opc_Client.ViewModel
{
    public class NetworkLevelViewModel : ViewModelBase, ITreeViewItemViewModel
    {
        private string networkLevel;
        private ObservableCollection<ITreeViewItemViewModel> children;
        private ITreeViewItemViewModel parent;
        private bool isSelected;
        private bool isExpanded;

        public ObservableCollection<ITreeViewItemViewModel> Children
        {
            get => children;
            set => Set(nameof(Children), ref children, value);
        }

        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                Set(nameof(IsExpanded), ref isExpanded, value);
                TreeViewItemViewModelHelpers.ExpandCheck(this, parent, LoadChildrenAsync);
            }
        }

        public bool IsSelected
        {
            get => isSelected;
            set => Set(nameof(IsSelected), ref isSelected, value);
        }

        public bool HasDummyChild
        {
            get => Children.Count == 1 && Children[0] == null;
        }

        public string NetworkLevel
        {
            get => networkLevel;
            set => Set(nameof(NetworkLevel), ref networkLevel, value);
        }

        public NetworkLevelViewModel(string name, ITreeViewItemViewModel _parent, bool loadChildren)
        {
            NetworkLevel = name;
            parent = _parent;
            TreeViewItemViewModelHelpers.InitializeChildren(this, loadChildren);
        }

        protected async Task LoadChildrenAsync()
        {
            if (NetworkLevel == "localhost")
                return;
            var temp = await NetworkBrowser.GetComputersOnNetworkAsync();
            Children = new ObservableCollection<ITreeViewItemViewModel>(temp.Select((a) => new OpcComputer(a, this, true)));
        }
    }
}
