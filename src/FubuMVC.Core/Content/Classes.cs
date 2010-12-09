using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Packaging;

namespace FubuMVC.Core.Content
{
    public interface IImageUrlResolver
    {
        string UrlFor(string name);
    }

    public interface IWebContentFileSystem
    {
        bool ContentFileExists(string folder, string fileName);
    }

    // TODO -- need to register this little monkey
    public class WebContentFileSystem : IWebContentFileSystem
    {
        public bool ContentFileExists(string folder, string fileName)
        {
            throw new NotImplementedException();
        }
    }

    // Be easier if this were built into FubuBootstrapper
    // Also be nice if we had the IStartupAction business going
    public class PackageFolderActivator : IPackageActivator
    {
        private readonly IPackagedImageUrlResolver _resolver;

        public PackageFolderActivator(IPackagedImageUrlResolver resolver)
        {
            _resolver = resolver;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            throw new NotImplementedException();
        }
    }

    public interface IPackagedImageUrlResolver
    {
        void RegisterDirectory(string directory);
    }

    public class PackagedImageUrlResolver : IImageUrlResolver, IPackagedImageUrlResolver
    {
        private readonly IFileSystem _fileSystem;
        private readonly IList<string> _directories = new List<string>();

        public PackagedImageUrlResolver(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void RegisterDirectory(string directory)
        {
            _directories.Add(directory);
        }

        public string UrlFor(string name)
        {
            throw new NotImplementedException();
        }
    }

    public class DefaultImageUrlResolver : IImageUrlResolver
    {
        private readonly IWebContentFileSystem _fileSystem;
        public static readonly string IMAGE_LOCATION = "~/content/images/";

        public DefaultImageUrlResolver(IWebContentFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string UrlFor(string name)
        {
            throw new NotImplementedException();
        }
    }

    public class ContentRegistryCache : IContentRegistry
    {
        private readonly IEnumerable<IImageUrlResolver> _imageResolvers;
        private readonly Cache<string, string> _urls = new Cache<string,string>();

        public ContentRegistryCache(IEnumerable<IImageUrlResolver> imageResolvers)
        {
            _imageResolvers = imageResolvers;

            _urls.OnMissing = name =>
            {
                var url = _imageResolvers.FirstValue(x => x.UrlFor(name))
                          ?? DefaultImageUrlResolver.IMAGE_LOCATION + name;

                return url.ToAbsoluteUrl();
            };
        }


        public string ImageUrl(string name)
        {
            return _urls[name];
        }

        public string CssUrl(string name)
        {
            throw new NotImplementedException();
        }
    }

    public interface ICssUrlResolver
    {
        string UrlFor(string name);
    }

    public interface IContentRegistry
    {
        string ImageUrl(string name);
        string CssUrl(string name);
    }
}