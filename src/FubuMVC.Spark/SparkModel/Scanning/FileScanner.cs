using System;
using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Spark.SparkModel.Scanning
{
    public interface IFileScanner
    {
        void Scan(ScanRequest request);
    }

    public class FileScanner : IFileScanner
    {
        private readonly IFileSystem _fileSystem;
        private IList<string> _scannedDirectories;

        public FileScanner() : this(new FileSystem()) {}
        public FileScanner(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Scan(ScanRequest request)
        {
            var fileSet = new FileSet { Include = request.Filters, DeepSearch = false };            
            _scannedDirectories = new List<string>();
            request.Roots.Each(root => scan(root, root, fileSet, request.OnFound));
        }

        private void scan(string root, string directory, FileSet fileSet, Action<FileFound> onFound)
        {
            if (alreadyScannedOrNonexistent(directory)) { return; }

            _scannedDirectories.Add(directory);

            _fileSystem.ChildDirectoriesFor(directory)
                .Each(dir => scan(root, dir, fileSet, onFound));

            _fileSystem.FindFiles(directory, fileSet)
                .Each(file => onFound(new FileFound(file, root, directory)));
        }

        private bool alreadyScannedOrNonexistent(string path)
        {
            return _scannedDirectories.Contains(path) || !_fileSystem.DirectoryExists(path);
        }
    }
}