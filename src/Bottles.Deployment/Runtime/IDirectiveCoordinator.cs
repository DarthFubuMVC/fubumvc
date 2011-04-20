using System.Collections.Generic;

namespace Bottles.Deployment.Runtime
{
    public interface IDirectiveCoordinator
    {
        void Initialize(IEnumerable<HostManifest> hosts);
        void Deploy(IEnumerable<HostManifest> hosts);
        void Finish(IEnumerable<HostManifest> hosts);
    }
}