using System.Collections.Generic;
using System.Linq;
using Bottles;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Packaging;
using FubuMVC.Spark.SparkModel.Scanning;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkItemFinder
    {
        IEnumerable<SparkItem> FindInHost();
        IEnumerable<SparkItem> FindInPackages();
    }

    public class SparkItemFinder : ISparkItemFinder
    {
        private readonly IFileScanner _fileScanner;
        private readonly IEnumerable<IPackageInfo> _packages;
        private CompositeAction<ScanRequest> _requestConfig;
        private string _hostPath;
        
        public SparkItemFinder() : this(new FileScanner(), PackageRegistry.Packages) { }
        public SparkItemFinder(IFileScanner fileScanner, IEnumerable<IPackageInfo> packages)
        {
            _fileScanner = fileScanner;
            _packages = packages;
            _requestConfig = new CompositeAction<ScanRequest>();
            
            Include("*spark");
            Include("bindings.xml");
        }

        public string HostPath
        {
            get { return _hostPath ?? "~/".ToPhysicalPath(); } 
            set { _hostPath = value; }
        }

        public IEnumerable<SparkItem> FindInHost()
        {
            var items = new List<SparkItem>();
            var root = new SparkRoot
            {
                Origin = Constants.HostOrigin, 
                Path = HostPath
            };

            var request = buildRequest(items, root);
            // NOTE : Brittle? Would be better if we could get the full path from fubu.
            request.ExcludeDirectory(FubuMvcPackageFacility.FubuPackagesFolder);
            request.ExcludeDirectory(FubuMvcPackageFacility.FubuContentFolder);
            
            _fileScanner.Scan(request);
            
            return items;
        }

        public IEnumerable<SparkItem> FindInPackages()
        {
            var items = new List<SparkItem>();
            var roots = packageRoots(_packages).ToArray();
            var request = buildRequest(items, roots);
            
            _fileScanner.Scan(request);
            
            return items;
        }

        public void Include(string filter)
        {
            _requestConfig += r => r.Include(filter);
        }

        private static IEnumerable<SparkRoot> packageRoots(IEnumerable<IPackageInfo> packages)
        {
            var packageRoots = new List<SparkRoot>();
            foreach (var package in packages)
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

        private ScanRequest buildRequest(ICollection<SparkItem> files, params SparkRoot[] sparkRoots)
        {
            var request = new ScanRequest();
            _requestConfig.Do(request);
            
            sparkRoots.Each(r => request.AddRoot(r.Path));
            request.AddHandler(fileFound =>
            {
                var origin = sparkRoots.First(x => x.Path == fileFound.Root).Origin;
                var sparkFile = new SparkItem(fileFound.Path, fileFound.Root, origin);                
                files.Add(sparkFile);
            });

            return request;
        }
    }      
}