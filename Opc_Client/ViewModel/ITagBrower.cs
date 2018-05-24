using Opc_Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;

namespace Opc_Client.ViewModel
{
    public interface ITagBrower : ITreeViewItemViewModel
    {
        IOpcTagModel SelectedItem { get; set; }
    }
}
