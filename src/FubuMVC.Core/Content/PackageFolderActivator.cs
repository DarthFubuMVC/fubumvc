using System;
using System.Collections.Generic;
using System.IO;
using FubuMVC.Core.Packaging;
using FubuCore;

namespace FubuMVC.Core.Content
{
    public class PackageFolderActivator : IActivator
    {
        private readonly IPackagedImageUrlResolver _resolver;

        public PackageFolderActivator(IPackagedImageUrlResolver resolver)
        {
            _resolver = resolver;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            packages.Each(p => p.ForFolder(FubuMvcPackages.WebContentFolder, topFolder =>
            {
                var imagesFolder = Path.Combine(topFolder, "content\\images");

                log.Trace("Added folder '{0}' to the PackagedImageUrl list", imagesFolder);
                _resolver.RegisterDirectory(imagesFolder);
            }));
        }

        public override string ToString()
        {
            return
                "Scan and activate image, CSS, and JavaScript content in package web content folders ({0})".ToFormat(
                    GetType().Name);
        }
    }
}