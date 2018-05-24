using Workstation.ServiceModel.Ua;

namespace Opc_Client.Model
{
    public interface IOpcTagModel
    {
        ReferenceDescription Node { get; set; }
        DataValue Value { get; set; }
    }
}