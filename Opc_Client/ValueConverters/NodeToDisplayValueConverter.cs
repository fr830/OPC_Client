using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Workstation.ServiceModel.Ua;

namespace Opc_Client.ValueConverters
{
    class NodeToDisplayValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ReferenceDescription)
            {
                ReferenceDescription node = value as ReferenceDescription;
                switch (node.NodeClass)
                {
                    case NodeClass.Unspecified:
                        break;
                    case NodeClass.Object:
                        break;
                    case NodeClass.Variable:
                        return node.DisplayName.Text.Contains(':') ? (node.DisplayName.Text.Substring(0, node.DisplayName.Text.IndexOf(':'))) : node.DisplayName.Text;                        
                    case NodeClass.Method:
                        break;
                    case NodeClass.ObjectType:
                        break;
                    case NodeClass.VariableType:
                        break;
                    case NodeClass.ReferenceType:
                        break;
                    case NodeClass.DataType:
                        break;
                    case NodeClass.View:
                        break;
                    default:
                        break;
                }
                return (node.DisplayName.ToString());
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
