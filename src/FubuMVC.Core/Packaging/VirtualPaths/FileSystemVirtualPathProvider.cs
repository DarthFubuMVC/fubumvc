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
            var relativePath = getRelativePath(virtualPath);
            if (containsInvalidPathChars(relativePath)) return null;
            return _directories
                .Select(x => Path.Combine(x, relativePath))
                .FirstOrDefault(File.Exists);
        }

        // TODO: Move to FubuCore.StringExtensions
        private static readonly char[] InvalidPathchars = Path.GetInvalidPathChars();
        private static bool containsInvalidPathChars(string path)
        {
            return path.Any(x => InvalidPathchars.Contains(x));
        }

        public override bool FileExists(string virtualPath)
        {
            return _files[virtualPath].IsNotEmpty() || 
                   (Previous != null && Previous.FileExists(virtualPath));
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

        private string getRelativePath(string virtualPath)
        {
            var applicationVirtualPath = HostingEnvironment.ApplicationVirtualPath ?? "";
            var virtualPathLength = applicationVirtualPath.Length;

            var originalPath = virtualPath;
            if (originalPath.StartsWith(applicationVirtualPath, StringComparison.InvariantCultureIgnoreCase))
            {
                originalPath = originalPath.Substring(virtualPathLength);
            }

            return originalPath.Replace("~", "").TrimStart('/');
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