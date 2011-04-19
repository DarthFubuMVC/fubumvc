using System.Collections.Generic;

namespace Bottles.Deployment.Parsing
{
    public interface IProfileReader
    {
        IEnumerable<HostManifest> Read();
    }
}