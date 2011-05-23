using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;

namespace FubuMVC.Core.Content
{
    [MarkedForTermination("Can make this obsolete with more generic PackageFileActivator")]
    public class PackageFolderActivator : IActivator
    {
        private readonly IContentFolderService _contents;

        public PackageFolderActivator(IContentFolderService contents)
        {
            _contents = contents;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            packages.Each(p => p.ForFolder(BottleFiles.WebContentFolder, topFolder =>
            {
                var contentFolder = FileSystem.Combine(topFolder, "content");

                log.Trace("Added folder '{0}' to the package folder list", contentFolder);
                _contents.RegisterDirectory(contentFolder);
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