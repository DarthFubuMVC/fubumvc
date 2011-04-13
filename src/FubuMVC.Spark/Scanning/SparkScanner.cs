using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace FubuMVC.Spark.Scanning
{
    public interface ISparkScanner
    {
        IEnumerable<SparkFile> Scan(IEnumerable<SourcePath> roots);
    }

    public class SparkScanner : ISparkScanner
    {
        private readonly IFileSystem _fileSystem;
        private IList<string> _scannedDirectories;

        public SparkScanner(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
        public IEnumerable<SparkFile> Scan(IEnumerable<SourcePath> roots)
        {
            var sources = sortRoots(roots);
            var fileSet = createFileSet();

            _scannedDirectories = new List<string>();

            var scanResult = new List<SparkFile>();            
            sources.Each(root =>
            {
                Action<string> onFound = path => scanResult.Add(getSparkFile(root, path));
                scanDirectory(root.Path, fileSet, onFound);
            });

            return scanResult;
        }

        private static SparkFile getSparkFile(SourcePath source,string path)
        {
            return new SparkFile(path, source.Path, source.Origin)
            {
                Namespace = "" /*FIX*/,
                ViewModel = null /*FIX*/
            };
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
}