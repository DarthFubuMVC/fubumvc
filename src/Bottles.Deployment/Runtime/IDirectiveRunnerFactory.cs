using System.Collections.Generic;

namespace Bottles.Deployment.Runtime
{
    public interface IDirectiveRunnerFactory
    {
        IEnumerable<IDirectiveRunner> BuildRunners(IEnumerable<HostManifest> hosts);
    }
}