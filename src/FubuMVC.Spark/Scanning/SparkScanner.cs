using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Spark.Parsing;

namespace FubuMVC.Spark.Scanning
{
    public interface ISparkScanner
    {
        IEnumerable<SparkFile> Scan(IEnumerable<SourcePath> roots);
    }

    public class SparkScanner : ISparkScanner
    {
        private readonly IFileSystem _fileSystem;
        private readonly ISparkFileComposer _composer;
        private IList<string> _scannedDirectories;

        public SparkScanner(IFileSystem fileSystem, ISparkFileComposer composer)
        {
            _fileSystem = fileSystem;
            _composer = composer;
        }

        public IEnumerable<SparkFile> Scan(IEnumerable<SourcePath> roots)
        {
            var sources = sortRoots(roots);
            var fileSet = createFileSet();

            _scannedDirectories = new List<string>();

            var scanResult = new List<SparkFile>();
            sources.Each(root =>
            {
                Action<string> onFound = path => scanResult.Add(_composer.Compose(root, path));
                scanDirectory(root.Path, fileSet, onFound);
            });

            return scanResult;
        }

        private void scanDirectory(string path, FileSet fileSet, Action<string> onFound)
        {
            if (alreadyScannedOrNonexistent(path)) { return; }

            _scannedDirectories.Add(path);

            _fileSystem.ChildDirectoriesFor(path)
                .Each(dir => scanDirectory(dir, fileSet, onFound));

            _fileSystem.FindFiles(path, fileSet)
                .Each(onFound);
        }

        private bool alreadyScannedOrNonexistent(string path)
        {
            return _scannedDirectories.Contains(path) || !_fileSystem.DirectoryExists(path);
        }

        private static IEnumerable<SourcePath> sortRoots(IEnumerable<SourcePath> sources)
        {
            return sources
                .Select(p => new { Path = p, Depth = p.Path.Split(Path.DirectorySeparatorChar).Count() })
                .OrderByDescending(o => o.Depth)
                .Select(p => p.Path)
                .ToList();
        }

        private static FileSet createFileSet()
        {
            return new FileSet
            {
                // TODO: Make this configurable, but let's stay with this for now
                Include = "*.spark",
                DeepSearch = false
            };
        }
    }

    public interface ISparkFileComposer
    {
        SparkFile Compose(SourcePath source, string filePath);
    }

    public interface ISparkFileAlteration
    {
        void Alter(SparkFile file);
    }

    public class SparkFileComposer : ISparkFileComposer
    {
        private readonly IEnumerable<ISparkFileAlteration> _alterations;

        public SparkFileComposer(IEnumerable<ISparkFileAlteration> alterations)
        {
            _alterations = alterations;
        }

        public SparkFile Compose(SourcePath source, string filePath)
        {
            var file = new SparkFile(filePath, source.Path, source.Origin);

            _alterations.Each(x => x.Alter(file));

            return file;
        }
    }

    public class ViewModelAlteration : ISparkFileAlteration
    {
        private readonly IFileSystem _fileSystem;
        private readonly IViewModelTypeParser _parser;

        public ViewModelAlteration(IFileSystem fileSystem, IViewModelTypeParser parser)
        {
            _fileSystem = fileSystem;
            _parser = parser;
        }

        public void Alter(SparkFile file)
        {
            var content = _fileSystem.ReadStringFromFile(file.Path);
            var type = _parser.Parse(content);
            file.ViewModel = type;
        }
    }

    public class NamespaceAlteration : ISparkFileAlteration
    {
        public void Alter(SparkFile file)
        {
            //TODO: FIX THIS, INTRODUCE PROPER ALGORITHM
            if (file.ViewModel != null)
            {
                var assemblyName = file.ViewModel.Assembly.GetName().Name;

                var relativePath = file.Path.Replace(file.Root, "").TrimStart(Path.DirectorySeparatorChar);
                var parts = relativePath
                    .Split(Path.DirectorySeparatorChar)
                    .Reverse().Skip(1) // exclude file name [something.spark]
                    .Reverse();// swap back to the original order

                var relativeNamespace = string.Empty; 
                
                if(parts.Any())
                {
                    // joining each part with '.'
                    relativeNamespace = parts
                    .Aggregate((a, b) => a + "." + b);
                    relativeNamespace = "." + relativeNamespace;
                }

                var ns = assemblyName + relativeNamespace;
                file.Namespace = ns;
            }
        }
    }
}