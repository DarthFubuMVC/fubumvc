using System.Collections.Generic;

namespace Bottles.Deployment.Runtime
{
    public interface IDirectiveRunner
    {
        void Deploy(IEnumerable<HostManifest> hosts);
    }
}