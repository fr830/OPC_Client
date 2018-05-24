using GalaSoft.MvvmLight;
using Opc_Client.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;

namespace Opc_Client.ViewModel
{
    public class MainOpcDiscoveryBrowserViewModel : ViewModelBase, ITreeViewItemViewModel
    {
        private ITreeViewItemViewModel parent;
        private ObservableCollection<ITreeViewItemViewModel> children;
        public ApplicationDescription AppDescription;
        public DirectoryStore CertificateStore;
        private bool isExpanded;
        private bool isSelected;

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
            set => Set(nameof(IsSelected), ref isSelected, value);
        }

        public bool HasDummyChild
        {
            get => Children.Count == 1 && Children[0] == null;
        }

        public MainOpcDiscoveryBrowserViewModel() 
        {
            AppDescription = new ApplicationDescription()
            {
                ApplicationName = "OpcClient",
                ApplicationType = ApplicationType.Client,
                ApplicationUri = $"urn:{Dns.GetHostName()}:Workstation.StatusHmi"
            };
            parent = null;
            CertificateStore = new DirectoryStore(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Opc_Client", "pki"));

            Children = new ObservableCollection<ITreeViewItemViewModel> { new OpcComputer("localhost", null, true), new NetworkLevelViewModel("Network", null, true) };
        }

    }
}
