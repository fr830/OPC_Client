using Opc_Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;

namespace Opc_Client.Helpers
{
    public static class TreeViewItemViewModelHelpers
    {
        public static void InitializeChildren(ITreeViewItemViewModel vm, bool loadChildren)
        {
            if (loadChildren)
            {
                vm.Children = new ObservableCollection<ITreeViewItemViewModel>{null};
            }
        }

        public static void ExpandCheck(ITreeViewItemViewModel vm, ITreeViewItemViewModel parent, Func<Task> LoadChildrenAsync)
        {
            if (vm.IsExpanded && parent != null)
                parent.IsExpanded = true;

            if (vm.HasDummyChild)
            {
                vm.Children.Clear();
                Task.Run(async () => await LoadChildrenAsync());
            }
        }

        internal static void CheckForEndTag(ITreeViewItemViewModel vm, ReferenceDescription node)
        {           
            //if (displayName.Text.Contains(":"))
            //{
            //    Datatype = displayName.Text.Substring(displayName.Text.IndexOf(":") + 1);
            //    Name = displayName.Text.Substring(0, displayName.Text.IndexOf(":"));
            //    vm.Children.Clear();
            //}
            //else
            //{
            //    Datatype = string.Empty;
            //    Name = displayName;
            //}
        }
    }
}
