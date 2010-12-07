using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Diagnostics.Querying
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