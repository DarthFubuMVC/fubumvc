using System.Collections.Generic;
using Bottles.Deployment;

namespace Bottles.Tests.Deployment.Runtime
{
    public static class DataMother
    {
        public static HostManifest BuildHostManifest(string manifestName, params IDirective[] directives)
        {
            var hostManifest = new HostManifest(manifestName);
            ((List<IDirective>)hostManifest.AllDirectives).AddRange(directives);
            return hostManifest;
        }
    }
}