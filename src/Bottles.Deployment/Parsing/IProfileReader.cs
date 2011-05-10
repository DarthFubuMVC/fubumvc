using System.Collections.Generic;
using Bottles.Deployment.Runtime;

namespace Bottles.Deployment.Parsing
{
    public interface IProfileReader
    {
        IEnumerable<HostManifest> Read(DeploymentOptions options);
    }
}