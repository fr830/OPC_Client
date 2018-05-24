using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;
using System.Linq;
using OpcClientLibrary;
using GalaSoft.MvvmLight;
using Opc_Client.Helpers;

namespace Opc_Client.ViewModel
{
    public class OpcComputer : ViewModelBase, ITreeViewItemViewModel
    {
        private string computerName;
        private string port;
        private bool isSelected;
        private bool isExpanded;
        private ObservableCollection<ITreeViewItemViewModel> children;
        private ITreeViewItemViewModel parent;

        public OpcComputer(string name, ITreeViewItemViewModel _parent, bool loadChildren = false)
        {
            ComputerName = name;
            parent = _parent;
            TreeViewItemViewModelHelpers.InitializeChildren(this, loadChildren);
        }

        public string ComputerName
        {
            get => computerName == "localhost" ? "127.0.0.1" : computerName;
            set => Set(nameof(ComputerName), ref computerName, value);
        }
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

        public GetEndpointsResponse GetEndpoints { get; set; }

        public string Port
        {
            get => port??"62845";
            set => Set(nameof(Port), ref port, value);
        }
        
        protected async Task LoadChildrenAsync()
        {
            GetEndpoints = await OpcInterface.GetEndpointsAsync(ComputerName, Port);
            Children = new ObservableCollection<ITreeViewItemViewModel>(GetEndpoints.Endpoints.Select((a) => new EndpointViewModel(a, this, false)));
            
            //GetEndpoints = OpcInterface.GetEndpoints(ComputerName);
        }
    }

}
