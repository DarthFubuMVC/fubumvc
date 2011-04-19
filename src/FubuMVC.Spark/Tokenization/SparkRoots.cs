using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bottles;
using FubuCore;

namespace FubuMVC.Spark.Tokenization
{
    public class SparkRoot
    {
        public string Path { get; set; }
        public string Origin { get; set; }
    }

    public class SparkRoots : IEnumerable<SparkRoot>
    {
        private readonly IEnumerable<IPackageInfo> _packages;
        private readonly Lazy<IEnumerable<SparkRoot>> _roots;

        public SparkRoots() : this(PackageRegistry.Packages) {}
        public SparkRoots(IEnumerable<IPackageInfo> packages)
        {
            _packages = packages;
            _roots = new Lazy<IEnumerable<SparkRoot>>(allRoots);
        }

        public IEnumerator<SparkRoot> GetEnumerator()
        {
            return _roots.Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<SparkRoot> allRoots()
        {
            return packages().Concat(host());
        }

        private IEnumerable<SparkRoot> packages()
        {
            var packageRoots = new List<SparkRoot>();
            foreach (var package in _packages)
            {
                var pack = package;
                package.ForFolder(BottleFiles.WebContentFolder, path =>
                {
                    var root = new SparkRoot
                    {
                        Origin = pack.Name,
                        Path = path
                    };

                    packageRoots.Add(root);
                });
            }
            return packageRoots;
        }

        private IEnumerable<SparkRoot> host()
        {
            yield return new SparkRoot
            {
                Origin = Constants.HostOrigin,
                Path = "~/".ToPhysicalPath()
            };
        }
    }
}