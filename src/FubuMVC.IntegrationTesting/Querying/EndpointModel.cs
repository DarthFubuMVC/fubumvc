using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.IntegrationTesting.Querying
{
    public class EndpointModel 
    {
        public EndpointToken[] AllEndpoints { get; set; }

        public IEnumerable<EndpointToken> EndpointsForAssembly(string assemblyName)
        {
            return AllEndpoints.Where(x => x.IsFromAssembly(assemblyName));
        }
    }
}