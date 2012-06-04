using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;

namespace FubuMVC.IntegrationTesting.Querying
{
    public class EndpointModel : JsonMessage
    {
        public EndpointToken[] AllEndpoints { get; set; }

        public IEnumerable<EndpointToken> EndpointsForAssembly(string assemblyName)
        {
            return AllEndpoints.Where(x => x.IsFromAssembly(assemblyName));
        }
    }
}