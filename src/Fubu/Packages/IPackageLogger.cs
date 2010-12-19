using System.Collections.Generic;
using FubuMVC.Core.Packaging;

namespace Fubu.Packages
{
    public interface IPackageLogger
    {
        void WriteAssembliesNotFound(CreatePackageInput input, PackageManifest manifest, IEnumerable<string> candidates);
    }
}