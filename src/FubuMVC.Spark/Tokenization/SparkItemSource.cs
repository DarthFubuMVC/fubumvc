using System.Collections.Generic;
using System.Linq;
using Bottles;
using FubuCore;
using FubuMVC.Spark.Tokenization.Scanning;

namespace FubuMVC.Spark.Tokenization
{
    public interface ISparkItemSource
    {
        IEnumerable<SparkItem> SparkItems();
    }

    public class SparkItemSource : ISparkItemSource
    {
        private readonly IFileScanner _fileScanner;
        private readonly IEnumerable<IPackageInfo> _packages;

        public SparkItemSource() : this(new FileScanner(), PackageRegistry.Packages) {}
        public SparkItemSource(IFileScanner fileScanner, IEnumerable<IPackageInfo> packages)
        {
            _fileScanner = fileScanner;
            _packages = packages;
        }

        public IEnumerable<SparkItem> SparkItems()
        {
            var items = new List<SparkItem>();

            var request = buildRequest(items);
            _fileScanner.Scan(request);

            return items;
        }

        private ScanRequest buildRequest(ICollection<SparkItem> files)
        {
            var request = new ScanRequest();
            request.AddFileFilter("*.spark");

            var roots = rootSources().ToList();
            roots.Select(x => x.Path).Each(request.AddRoot);

            request.AddHandler(fileFound =>
            {
                var origin = roots.First(x => x.Path == fileFound.Root).Origin;
                var sparkFile = new SparkItem(fileFound.Path, fileFound.Root, origin);
                
                files.Add(sparkFile);
            });

            return request;
        }

        private IEnumerable<RootSource> rootSources()
        {
            var roots = new List<RootSource>();
            
            foreach (var package in _packages)
            {
                var pack = package;
                package.ForFolder(BottleFiles.WebContentFolder, file =>
                {
                    var root = new RootSource
                    {
                        Origin = pack.Name, 
                        Path = file
                    };

                    roots.Add(root);
                });
            }

            roots.Add(new RootSource
            {
                Origin = Constants.HostOrigin, 
                Path = "~/".ToPhysicalPath()
            });
            
            return roots;
        }

        #region Nested Type : RootSource
        
        public class RootSource
        {
            public string Path { get; set; }
            public string Origin { get; set; }
        }
        
        #endregion
    }
}