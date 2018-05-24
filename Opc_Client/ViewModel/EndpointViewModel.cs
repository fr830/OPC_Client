using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Opc_Client.Helpers;
using OpcClientLibrary;
using System.Collections.ObjectModel;
using Workstation.ServiceModel.Ua;

namespace Opc_Client.ViewModel
{
    public class EndpointViewModel : ViewModelBase, ITreeViewItemViewModel
    {
        private ITreeViewItemViewModel parent;
        private bool isExpanded;
        private bool isSelected;
        private ObservableCollection<ITreeViewItemViewModel> children;
        public EndpointViewModel(EndpointDescription endpoint, ITreeViewItemViewModel _parent, bool loadChildren = false)
        {
            Endpoint = endpoint;
            parent = _parent;
            TreeViewItemViewModelHelpers.InitializeChildren(this, loadChildren);            
        }

        private MainOpcDiscoveryBrowserViewModel mainOpc = SimpleIoc.Default.GetInstance<MainOpcDiscoveryBrowserViewModel>();
        public EndpointDescription Endpoint { get; private set; }

        public string EndpointUrl { get => Endpoint.EndpointUrl ?? "n/a"; }

        public string Security { get => Endpoint.SecurityMode.ToString(); }
        public OpcConnection Connection { get; set; }
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
                TreeViewItemViewModelHelpers.ExpandCheck(this, parent, null);
            }
        }

        public bool IsSelected
        {
            get => isSelected;
            set { Set(nameof(IsSelected), ref isSelected, value); Messenger.Default.Send<OpcConnection>(Connection); } 
        }

        public bool HasDummyChild
        {
            get => Children?.Count == 1 && Children?[0] == null;
        }
        private RelayCommand connect;

        /// <summary>
        /// Gets the Connect Command.
        /// </summary>
        public RelayCommand Connect
        {
            get
            {
                return connect
                    ?? (connect = new RelayCommand(
                    async () =>
                    {
                        Connection = new OpcConnection();
                        await Connection.ConnectAsync(Endpoint, mainOpc.AppDescription, mainOpc.CertificateStore);                        
                    }));
            }
        }
    }
}
