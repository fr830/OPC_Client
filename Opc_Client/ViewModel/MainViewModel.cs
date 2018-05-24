using GalaSoft.MvvmLight;
using Opc_Client.Model;
using Workstation.ServiceModel.Ua;

namespace Opc_Client.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private IOpcTagModel selectedNode;

        public IOpcTagModel SelectedNode
        {
            get => selectedNode; 
            set => Set(ref selectedNode, value); 
        }
        private BrowseRootViewModel opcBrowser;
        private OpcTagViewModel opcTagVM;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            opcBrowser = (new ViewModelLocator()).OpcBrowser;
            opcTagVM = (new ViewModelLocator()).OpcTagView;
            opcBrowser.PropertyChanged += SelectedNode_PropertyChanged;
        }

        private void SelectedNode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "SelectedItem")
            {
                opcTagVM.Tag = opcBrowser.SelectedItem;
            }
        }
    }
}