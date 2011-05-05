using System.Collections.Generic;
using System.IO;
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
        private CompositeAction<ScanRequest> _hostExcludes;
        private string _hostPath;
        
        public SparkItemFinder() : this(new FileScanner(), PackageRegistry.Packages) { }
        public SparkItemFinder(IFileScanner fileScanner, IEnumerable<IPackageInfo> packages)
        {
            _fileScanner = fileScanner;
            _packages = packages;
            _requestConfig = new CompositeAction<ScanRequest>();
            _hostExcludes = new CompositeAction<ScanRequest>();

            IncludeFile("*spark");
            IncludeFile("bindings.xml");

            ExcludeHostDirectory(FubuMvcPackageFacility.FubuPackagesFolder);
            ExcludeHostDirectory(FubuMvcPackageFacility.FubuPackagesFolder, FubuMvcPackageFacility.FubuContentFolder);
            ExcludeHostDirectory(FubuMvcPackageFacility.FubuContentFolder);
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
                Origin = FubuSparkConstants.HostOrigin, 
                Path = HostPath
            };

            var request = buildRequest(items, root);
            _hostExcludes.Do(request);
            
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

        public void IncludeFile(string filter)
        {
            _requestConfig += r => r.Include(filter);
        }

        public void ExcludeHostDirectory(string path)
        {
            _hostExcludes += r => r.ExcludeDirectory(Path.Combine(HostPath, path));
        }
        public void ExcludeHostDirectory(params string[] parts)
        {
            excludeDirectory(Path.Combine(HostPath, Path.Combine(parts)));
        }

        private void excludeDirectory(string path)
        {
            _hostExcludes += r => r.ExcludeDirectory(path);
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