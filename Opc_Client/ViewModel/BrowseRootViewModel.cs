using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Opc_Client.Helpers;
using Opc_Client.Model;
using OpcClientLibrary;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

namespace Opc_Client.ViewModel
{
    public class BrowseRootViewModel : ViewModelBase, ITagBrower
    {        
        private OpcConnection connection;
        private ITagBrower parent;
        private ObservableCollection<ITreeViewItemViewModel> children;
        private bool isSelected;
        private bool isExpanded;

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
            set => Set(nameof(SelectedItem), ref selectedItem, value);  // Add Event for the item changing....or see if the parent can subscribe to prop change
        }

        public BrowseRootViewModel()
        {
            Messenger.Default.Register<OpcConnection>(this, RefreshConnection);                        
        }        

        private void RefreshConnection(OpcConnection currentConnection)
        {
            connection = currentConnection;
            connection.Channel.Opened += ChannelOpenedEventHandler;        
        }

        private void ChannelOpenedEventHandler(object sender, EventArgs e)
        {
            if ((sender as UaTcpSessionChannel).State == CommunicationState.Opened)
            {
                Children = new ObservableCollection<ITreeViewItemViewModel>() { new BrowseTagsViewModel(new ReferenceDescription() { DisplayName = "Root" }, connection.Channel, this, true) };
            }
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

        private RelayCommand refreshCommand;

        /// <summary>
        /// Gets the RefreshCommand.
        /// </summary>
        public RelayCommand RefreshCommand
        {
            get
            {
                return refreshCommand
                    ?? (refreshCommand = new RelayCommand(
                    () =>
                    {
                        Children?.Clear();
                        Children = new ObservableCollection<ITreeViewItemViewModel>() { new BrowseTagsViewModel(new ReferenceDescription() { DisplayName = "Root" }, connection.Channel, this, true) };
                        SelectedItem = null;
                    }));
            }
        }
    }
}
