using System.Collections.Generic;
using Bottles;
using FubuCore;

namespace FubuMVC.Spark.Scanning
{
    // TODO : Perhaps just a basic source that you can register against.

    public interface IScanSource
    {
        IEnumerable<string> Paths();
    }

    public class WebRootSource : IScanSource
    {
        public IEnumerable<string> Paths()
        {
            yield return "~/".ToPhysicalPath();
        }
    }

    public class PackagesSource : IScanSource
    {
        private readonly IEnumerable<IPackageInfo> _packages;
        public PackagesSource(IEnumerable<IPackageInfo> packages)
        {
            _packages = packages;
        }

        public IEnumerable<string> Paths()
        {
            var roots = new List<string>();
            _packages.Each(pak => pak.ForFolder(BottleFiles.WebContentFolder, roots.Add));
            return roots;
        }
    }
}