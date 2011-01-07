using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Content
{
    public interface IContentFolderService
    {
        string FileNameFor(string contentFileName);
        bool FileExists(string contentFileName);
        void RegisterDirectory(string directory);
    }

    public class ContentFolderService : IContentFolderService
    {
        private readonly Cache<string, string> _fileNames = new Cache<string, string>();
        private readonly IFileSystem _fileSystem;
        private readonly IList<string> _directories = new List<string>();

        public ContentFolderService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;

            _fileNames.OnMissing = name =>
            {
                return _directories.FirstValue(dir =>
                {
                    return _fileSystem.FileExists(dir, name) ? FileSystem.Combine(dir, name) : null;
                });
            };
        }

        public void RegisterDirectory(string directory)
        {
            _directories.Add(directory);
        }

        public string FileNameFor(string contentFileName)
        {
            return _fileNames[contentFileName];
        }

        public bool FileExists(string contentFileName)
        {
            return FileNameFor(contentFileName) != null;
        }
    }
}