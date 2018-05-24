using GalaSoft.MvvmLight;
using Opc_Client.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;

namespace Opc_Client.ViewModel
{
    public class OpcTagViewModel : ViewModelBase
    {
        private IOpcTagModel tag;
        public IOpcTagModel Tag { get => tag; set => Set(ref tag, value); }
    }
}
