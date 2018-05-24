using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;

namespace OpcClientLibrary
{
    public static class OpcInterface
    {
        private static ILoggerFactory loggerFactory = new LoggerFactory();

        public static async Task<GetEndpointsResponse> GetEndpointsAsync(string endpoint, string port)
        {
            var temp = await UaTcpDiscoveryService.GetEndpointsAsync(new GetEndpointsRequest() { EndpointUrl = "opc.tcp://" + endpoint + ":" + port }, loggerFactory);

            return temp;
        }

        public static GetEndpointsResponse GetEndpoints(string endpoint, string port)
        {
            var temp = UaTcpDiscoveryService.GetEndpointsAsync(new GetEndpointsRequest() { EndpointUrl = "opc.tcp://" + endpoint + ":" + port }, loggerFactory);
            return temp.Result;
        }

    }
}
