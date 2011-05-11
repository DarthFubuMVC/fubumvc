using System;
using Bottles.Deployers.Iis;
using Bottles.Deployment;
using Bottles.Deployment.Directives;
using Bottles.Deployment.Runtime;
using Bottles.Deployment.Runtime.Content;
using Bottles.Diagnostics;

namespace FubuMVC.Deployers
{
    public class FubuIisWebsiteDeployer : IDeployer<Website>
    {
        private readonly IBottleMover _bottleMover;

        public FubuIisWebsiteDeployer(IBottleMover bottleMover)
        {
            _bottleMover = bottleMover;
        }

        public void Execute(Website website, HostManifest host, IPackageLog log)
        {
            // TODO -- more logging!!!!
            
            new IisWebsiteCreator().Create(website);

            var destination = new FubuBottleDestination(website.VDirPhysicalPath);
            _bottleMover.Move(destination, host.BottleReferences);
        }
    }
}
