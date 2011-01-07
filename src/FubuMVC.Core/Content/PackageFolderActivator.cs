using System;
using System.Collections.Generic;
using System.IO;
using FubuMVC.Core.Packaging;
using FubuCore;

namespace FubuMVC.Core.Content
{
    public class PackageFolderActivator : IActivator
    {
        private readonly IContentFolderService _contents;

        public PackageFolderActivator(IContentFolderService contents)
        {
            _contents = contents;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            packages.Each(p => p.ForFolder(FubuMvcPackages.WebContentFolder, topFolder =>
            {
                var imagesFolder = FileSystem.Combine(topFolder, "content", "images");

                log.Trace("Added folder '{0}' to the PackagedImageUrl list", imagesFolder);
                _contents.RegisterDirectory(imagesFolder);
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