using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Content
{
    public class PackagedImageUrlResolver : IImageUrlResolver, IPackagedImageUrlResolver
    {
        private readonly IFileSystem _fileSystem;
        private readonly IList<string> _directories = new List<string>();
        private readonly Cache<string, string> _fileNames = new Cache<string, string>();

        public PackagedImageUrlResolver(IFileSystem fileSystem)
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

        public string FileNameFor(string name)
        {
            return _fileNames[name];
        }

        public string UrlFor(string name)
        {
            if (_fileNames[name] == null) return null;

            return "~/_images/" + name.TrimStart('/');
        }
    }
}