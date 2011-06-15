using System;
using Bottles.Deployers.Iis;
using Bottles.Deployment;
using Bottles.Deployment.Runtime;
using Bottles.Deployment.Runtime.Content;
using Bottles.Diagnostics;

namespace FubuMVC.Deployers
{
    public class FubuWebsite : Website{}

    public class FubuIisWebsiteDeployer : IDeployer<FubuWebsite>
    {
        private readonly IBottleMover _bottleMover;

        public FubuIisWebsiteDeployer(IBottleMover bottleMover)
        {
            _bottleMover = bottleMover;
        }

        public void Execute(FubuWebsite website, HostManifest host, IPackageLog log)
        {
            // TODO -- more logging!!!!

            new IisWebsiteCreator().Create(website);
            
            var destination = new FubuBottleDestination(website.VDirPhysicalPath);
            _bottleMover.Move(log, destination, host.BottleReferences);
        }

        public string GetDescription(FubuWebsite directive)
        {
            return "Installing a FubuMVC application at " + directive.VDirPhysicalPath;
        }
    }
}
