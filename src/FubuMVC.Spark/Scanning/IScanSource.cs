using System.Collections.Generic;
using Bottles;
using FubuCore;

namespace FubuMVC.Spark.Scanning
{
    // TODO : Perhaps just a basic source that you can register against.

    public enum SourceCategory
    {
        Host,
        Package
    }

    public class SourcePath
    {
        public string Path { get; set; }
        public SourceCategory Category { get; set; }
        public string Origin { get; set; }
    }

    public interface IScanSource
    {
        IEnumerable<SourcePath> Paths();
    }

    public class WebRootSource : IScanSource
    {
        public IEnumerable<SourcePath> Paths()
        {
            yield return new SourcePath
            {
                Path = "~/".ToPhysicalPath(), 
                Category = SourceCategory.Host, 
                Origin = ""
            };
        }
    }

    public class PackagesSource : IScanSource
    {
        private readonly IEnumerable<IPackageInfo> _packages;

        public PackagesSource(IEnumerable<IPackageInfo> packages)
        {
            _packages = packages;
        }

        public IEnumerable<SourcePath> Paths()
        {
            var folder = BottleFiles.WebContentFolder;

            foreach (var package in _packages)
            {
                SourcePath sourcePath = null;
                package.ForFolder(folder, f => sourcePath = new SourcePath { Path = f });
                if (sourcePath == null)
                {
                    continue;
                }
                sourcePath.Category = SourceCategory.Package;
                sourcePath.Origin = package.Name;
                yield return sourcePath;
            }
        }
    }
}