using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace FubuMVC.Spark.Scanning
{
    public interface ISparkScanner
    {
        IEnumerable<SparkFile> Scan(IEnumerable<string> roots);
    }

    public class SparkScanner : ISparkScanner
    {
        private readonly IFileSystem _fileSystem;
        private IList<string> _scannedDirectories;

        public SparkScanner(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
        public IEnumerable<SparkFile> Scan(IEnumerable<string> roots)
        {
            var sources = sortRoots(roots);
            var fileSet = createFileSet();

            _scannedDirectories = new List<string>();

            var scanResult = new List<SparkFile>();            
            sources.Each(root =>
            {
                Action<string> onFound = path => scanResult.Add(new SparkFile(path, root));
                scanDirectory(root, fileSet, onFound);
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

        private IEnumerable<string> sortRoots(IEnumerable<string> paths)
        {
            return paths
                .Select(p => new { Path = p, Depth = p.Split(Path.DirectorySeparatorChar).Count() })
                .OrderByDescending(o => o.Depth)
                .Select(p => p.Path)
                .ToList();
        }

        private FileSet createFileSet()
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