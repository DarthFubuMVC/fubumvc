using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bottles;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Spark.Scanning
{
    public interface IFileScanner
    {
        void Scan(ScanRequest request);
    }

    public class ScanRequest
    {
        private readonly List<string> _roots;
        private readonly List<string> _filter;
        private CompositeAction<FileFound> _onFound;
        public ScanRequest()
        {
            _roots = new List<string>();
            _filter = new List<string>();
            _onFound = new CompositeAction<FileFound>();
        }

        public IEnumerable<string> Roots { get { return _roots; } }
        public string Filters { get { return _filter.Join(";"); } }

        public void AddRoot(string root)
        {
            _roots.Add(root);
        }
        public void AddFileFilter(string filter)
        {
            _filter.Add(filter);
        }
        public void AddHandler(Action<FileFound> handler)
        {
            _onFound += handler;
        }
        public void OnFound(FileFound file)
        {
            _onFound.Do(file);
        }
    }

    public class FileFound
    {
        public string Root { get; set; }
        public string Directory { get; set; }
        public string Path { get; set; }
    }

    public class FileScanner : IFileScanner
    {
        private readonly IFileSystem _fileSystem;
        private IList<string> _scannedDirectories;

        public FileScanner(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Scan(ScanRequest request)
        {
            var sources = sortPaths(request.Roots);
            var fileSet = new FileSet { Include = request.Filters, DeepSearch = false };
            _scannedDirectories = new List<string>();

            sources.Each(root => scan(root, root, fileSet, request.OnFound));
        }

        private void scan(string root, string directory, FileSet fileSet, Action<FileFound> onFound)
        {
            if (alreadyScannedOrNonexistent(directory)) { return; }

            _scannedDirectories.Add(directory);

            _fileSystem.ChildDirectoriesFor(directory)
                .Each(dir => scan(root, dir, fileSet, onFound));

            _fileSystem.FindFiles(directory, fileSet)
                .Each(file => onFound(new FileFound { Path = file, Root = root, Directory = directory }));
        }

        private static IEnumerable<string> sortPaths(IEnumerable<string> paths)
        {
            return paths
                .Select(p => new { Path = p, Depth = p.Split(Path.DirectorySeparatorChar).Count() })
                .OrderByDescending(o => o.Depth)
                .Select(p => p.Path).ToList();
        }
        private bool alreadyScannedOrNonexistent(string path)
        {
            return _scannedDirectories.Contains(path) || !_fileSystem.DirectoryExists(path);
        }
    }

    public interface ISparkFileComposer
    {
       IEnumerable<SparkFile> Compose();
    }

    public interface ISparkFileAlteration
    {
        void Alter(SparkFile file);
    }

    public class SparkFileComposer : ISparkFileComposer
    {
        private readonly IFileScanner _fileScanner;
        private readonly IEnumerable<ISparkFileAlteration> _alterations;
        private readonly IEnumerable<IPackageInfo> _packages;

        public SparkFileComposer(IFileScanner fileScanner, IEnumerable<ISparkFileAlteration> alterations,IEnumerable<IPackageInfo> packages)
        {
            _fileScanner = fileScanner;
            _alterations = alterations;
            _packages = packages;
        }

        public IEnumerable<SparkFile> Compose()
        {
            var files = new List<SparkFile>();

            var request = buildRequest(files);

            _fileScanner.Scan(request);

            files.Each(x => _alterations.Each(a => a.Alter(x)));

            return files;
        }

        private ScanRequest buildRequest(ICollection<SparkFile> files)
        {
            var request = new ScanRequest();
            var roots = rootSources().ToList();

            request.AddFileFilter("*.spark");
            roots.Select(x => x.Path).Each(request.AddRoot);

            request.AddHandler(file =>
                                   {
                                       var origin = roots.First(x => x.Path == file.Root).Origin;
                                       files.Add(new SparkFile(file.Path, file.Root, origin));
                                   });
            return request;
        }

        private IEnumerable<RootSource> rootSources()
        {
            var roots = new List<RootSource>();
            foreach (var package in _packages)
            {
                var local = package;
                package.ForFolder(BottleFiles.WebContentFolder, file =>
                {
                    var rootSource = new RootSource{Origin = local.Name, Path = file};
                    roots.Add(rootSource);
                });
            }
            roots.Add(new RootSource { Origin = "Host", Path = "~/".ToPhysicalPath() });
            return roots;
        }

        private class RootSource
        {
            public string Path { get; set; }
            public string Origin { get; set; }
        }
    }

    public class ViewModelAlteration : ISparkFileAlteration
    {
        private readonly IViewModelTypeResolver _resolver;

        public ViewModelAlteration(IViewModelTypeResolver resolver)
        {
            _resolver = resolver;
        }

        public void Alter(SparkFile file)
        {
            file.ViewModel = _resolver.Resolve(file);
        }
    }

    public class NamespaceAlteration : ISparkFileAlteration
    {
        private readonly INamespaceResolver _resolver;

        public NamespaceAlteration(INamespaceResolver resolver)
        {
            _resolver = resolver;
        }

        public void Alter(SparkFile file)
        {
            file.Namespace = _resolver.Resolve(file);
        }
    }
}