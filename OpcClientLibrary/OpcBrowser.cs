using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;
using System.Linq;
using System.Collections.Generic;
using System;

namespace OpcClientLibrary
{
    public static class OpcBrowser
    {
        public static async Task<IEnumerable<ReferenceDescription[]>> BrowseOpcServerAsync(UaTcpSessionChannel channel, NodeId BrowseRoot)
        {
            BrowseRequest browseRequest = new BrowseRequest()
            {
                NodesToBrowse = new BrowseDescription[] { new BrowseDescription() { NodeId = BrowseRoot, BrowseDirection = BrowseDirection.Forward, ReferenceTypeId = NodeId.Parse(ReferenceTypeIds.HierarchicalReferences), NodeClassMask = (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, IncludeSubtypes = true, ResultMask = (uint)BrowseResultMask.All } },
            };
            var browseResponse = await channel.BrowseAsync(browseRequest);
            var temp = browseResponse.Results.Select((a) => a.References ?? new ReferenceDescription[0]);
            return temp;
        }

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }

}
