using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;

namespace Opc_Client.Model
{
    public class OpcTagModel : ViewModelBase, IOpcTagModel
    {
        private ReferenceDescription node;
        private DataValue value;
        public ReferenceDescription Node { get=>node; set=>Set(ref node, value); }
        public DataValue Value { get=>value; set=>Set(ref this.value, value); }
    }
}
