using System.Collections.Generic;
using Bottles;
using FubuCore;

namespace FubuMVC.Spark.Scanning
{
    // NOTE: Later on, we could do this a bit more elegant by having a ScanRequest on Scanner instead that would have
    //  - Filter
    //  - ScanResult<T> -> being able to do projection of result.
    //  - Possibly OnFound

    //    Then go by a CompositeScanRequest with a Register(Action<ScanRequest> configure):
    //  - if no Filter being set, the get parent's.
    //  - ScanResult of composite would get results from children.

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
                Origin = SourceCategory.Host.ToString()
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