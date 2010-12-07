using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FubuMVC.UI.Scripts.Registration
{
    public class FilePool
    {
        private readonly List<DirectoryInfo> _directories = new List<DirectoryInfo>();   
        private readonly IList<FileInfo> _files = new List<FileInfo>();
        private bool _scanned;

        private IList<FileInfo> files
        {
            get
            {
                if(!_scanned)
                {
                    _scanned = true;
                    _files.AddRange(_directories.SelectMany(info => info.GetFiles("*.js", SearchOption.AllDirectories)));
                }

                return _files;
            }
        }

        public void AddDirectory(DirectoryInfo directory)
        {
            _directories.Add(directory);
        }

        public void AddFile(FileInfo file)
        {
            _files.Fill(file);
        }

        public IEnumerable<FileInfo> FilesMatching(Func<FileInfo, bool> filter)
        {
            return files.Where(filter);
        }
    }
}