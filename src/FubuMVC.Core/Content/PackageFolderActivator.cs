using System.Collections.Generic;
using System.IO;
using FubuMVC.Core.Packaging;

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
    }
}