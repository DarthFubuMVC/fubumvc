using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Caching;
using System.Web.Hosting;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Packaging.VirtualPaths
{
    public class FileSystemVirtualPathProvider : VirtualPathProvider
    {
        private readonly IList<string> _directories = new List<string>();
        private readonly Cache<string, string> _files = new Cache<string, string>();

        public FileSystemVirtualPathProvider()
        {
            _files.OnMissing = findPhysicalPath;
        }

        public void RegisterContentDirectory(string directory)
        {
            _directories.Add(directory);
        }

        private string findPhysicalPath(string virtualPath)
        {
            return _directories
                .Select(x => getPhysicalPathToFile(virtualPath, x))
                .FirstOrDefault(File.Exists);
        }

        public override bool FileExists(string virtualPath)
        {
            var fileExists = _files[virtualPath].IsNotEmpty();
            return fileExists ? true : Previous.FileExists(virtualPath);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            var filePath = _files[virtualPath];
            if (!File.Exists(filePath))
            {
                return Previous.GetFile(virtualPath);
            }

            return new FileSystemVirtualFile(virtualPath, filePath);
        }

        public override bool DirectoryExists(string virtualDir)
        {
            var physicalPath = _files[virtualDir];
            if (physicalPath.IsNotEmpty())
            {
                return true;
            }

            return Previous.DirectoryExists(virtualDir);
        }

        public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            var physicalPath = _files[virtualPath];
            if (physicalPath.IsNotEmpty())
            {
                var baseHash = base.GetFileHash(virtualPath, virtualPathDependencies);
                return baseHash + File.GetLastWriteTimeUtc(physicalPath).Ticks;
            }

            return Previous.GetFileHash(virtualPath, virtualPathDependencies);
        }

        private string getPhysicalPathToFile(string virtualPath, string directory)
        {
            var applicationVirtualPath = HostingEnvironment.ApplicationVirtualPath;
            var virtualPathLength = applicationVirtualPath.Length;

            var originalPath = virtualPath;
            if (originalPath.StartsWith(applicationVirtualPath, StringComparison.InvariantCultureIgnoreCase))
            {
                originalPath = originalPath.Substring(virtualPathLength);
            }

            var physicalPath = originalPath.Replace("~", "").TrimStart('/');

            return Path.Combine(directory, physicalPath);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (virtualPathDependencies == null)
            {
                return null;
            }


            var knownFiles = new List<string>();
            var unknownVirtualPaths = new List<string>();

            foreach (string dependency in virtualPathDependencies)
            {
                var file = _files[dependency];
                if (file == null)
                {
                    unknownVirtualPaths.Add(dependency);
                }
                else
                {
                    knownFiles.Add(file);
                }
            }

            var otherDependencies = Previous.GetCacheDependency(virtualPath, unknownVirtualPaths, utcStart);
            

            return new CacheDependency(knownFiles.ToArray(), new string[0], otherDependencies);
        }
    }
}