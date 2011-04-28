using System;
using System.Threading;
using Bottles.Exploding;
using Bottles.Services;
using FubuCore;

namespace Bottles.Host
{
    class BottleHost
    {
        private IBottleAwareService _svc;
        private readonly IPackageExploder _exploder;
        private readonly IFileSystem _fileSystem;

        public BottleHost(IPackageExploder exploder, IFileSystem fileSystem)
        {
            _exploder = exploder;
            _fileSystem = fileSystem;
        }

        public void Start()
        {
            //sets bottles to look for things in '~/packages'
            BottleFiles.PackagesFolder = "packages";

            var manifest = _fileSystem.LoadFromFile<ServicePackageManifest>("svc", ServicePackageManifest.FILE);

            var type = Type.GetType(manifest.Bootstrapper, true, true);

            //guard clauses here

            _svc = (IBottleAwareService) Activator.CreateInstance(type);

            //this is done so that start can return, as 'LoadPackages' may take some time.
            ThreadPool.QueueUserWorkItem(cb =>
            {
                int shutUpResharper = 0;

                PackageRegistry.LoadPackages(pkg =>
                {
                    pkg.Loader(new BottleHostLoader(_fileSystem, _exploder));

                    pkg.Bootstrapper(_svc);
                });
            });

        }

        public void Stop()
        {
            _svc.Stop();
        }
    }
}