using GalaSoft.MvvmLight;
using Opc_Client.Helpers;
using Opc_Client.Model;
using OpcClientLibrary;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

namespace Opc_Client.ViewModel
{
    public class BrowseTagsViewModel : ViewModelBase, ITagBrower
    {
        private UaTcpSessionChannel channel;
        private IOpcTagModel node;
        private bool isSelected;
        private bool isExpanded;
        private ObservableCollection<ITreeViewItemViewModel> children;
        private ITagBrower parent;

        public BrowseTagsViewModel(ReferenceDescription node, UaTcpSessionChannel currentChannel, ITagBrower _parent, bool loadChildren = false)
        {
            parent = _parent;
            TreeViewItemViewModelHelpers.InitializeChildren(this, loadChildren);
            //TreeViewItemViewModelHelpers.CheckForEndTag(this, node);          
            Node = new OpcTagModel() { Node = node };
            channel = currentChannel;            
        }
        public ObservableCollection<ITreeViewItemViewModel> Children
        {
            get => children;
            set => Set(nameof(Children), ref children, value);
        }

        public IOpcTagModel Node
        {
            get => node;
            set => Set(ref node, value);
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
            set
            {
                Set(nameof(IsSelected), ref isSelected, value);
                if (node.Node.NodeClass == NodeClass.Variable)
                {
                    var a = Task.Run(async() => await channel.ReadAsync(new ReadRequest() { NodesToRead = new ReadValueId[] { new ReadValueId { NodeId = node.Node.NodeId.NodeId, AttributeId=AttributeIds.Value} } }));
                    foreach (var result in a.Result.Results)
                    {
                        node.Value = result;
                    }
                    
                }
                
                parent.SelectedItem = node;
            }       
        }

        public bool HasDummyChild
        {
            get => Children?.Count == 1 && Children?[0] == null;
        }

        /// <summary>
        /// The <see cref="SelectedItem" /> property's name.
        /// </summary>
        public const string SelectedItemPropertyName = "SelectedItem";

        private IOpcTagModel selectedItem;

        /// <summary>
        /// Sets and gets the SelectedItem property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public IOpcTagModel SelectedItem
        {
            get => selectedItem;
            set { Set(nameof(SelectedItem), ref selectedItem, value); parent.SelectedItem = value; }
        }

        protected async Task LoadChildrenAsync()
        {
            var temp = await OpcBrowser.BrowseOpcServerAsync(channel, node.Node.DisplayName.ToString().ToLower() == "root"? NodeId.Parse(ObjectIds.RootFolder) : Node.Node.NodeId.NodeId);
            Children = new ObservableCollection<ITreeViewItemViewModel>(temp.First().Select((a) => new BrowseTagsViewModel(a, channel, this, a.NodeClass!=NodeClass.Variable)));
            
        }        
    }
}
