using System;
using System.Collections.Generic;
using Bottles.Deployment;
using Bottles.Deployment.Diagnostics;
using Bottles.Deployment.Directives;
using Bottles.Deployment.Runtime;
using Bottles.Diagnostics;
using FubuCore;
using Microsoft.Web.Administration;

namespace Bottles.Deployers.Iis
{
    public class IisFubuDeployer : IDeployer<FubuWebsite>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IBottleRepository _bottles;

        public IisFubuDeployer(IFileSystem fileSystem, IBottleRepository bottles)
        {
            _fileSystem = fileSystem;
            _bottles = bottles;
        }

        public void Execute(FubuWebsite directive, HostManifest host, IPackageLog log)
        {
            //currenly only IIS 7
            using (var iisManager = new ServerManager())
            {
                var pool = iisManager.CreateAppPool(directive.AppPool);
                pool.ManagedRuntimeVersion = "v4.0";
                pool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
                pool.Enable32BitAppOnWin64 = false;

                if (directive.HasCredentials())
                {
                    pool.ProcessModel.UserName = directive.Username;
                    pool.ProcessModel.Password = directive.Password;
                }

                var site = iisManager.CreateSite(directive.WebsiteName, directive.WebsitePhysicalPath, directive.Port);


                var app = site.CreateApplication(directive.VDir, directive.VDirPhysicalPath);
                app.ApplicationPoolName = directive.AppPool;

                //flush the changes so that we can now tweak them.
                iisManager.CommitChanges();

                app.DirectoryBrowsing(directive.DirectoryBrowsing);

                //app.AnonAuthentication(direc.AnonAuth);
                //app.BasicAuthentication(direc.BasicAuth);
                //app.WindowsAuthentication(direc.WindowsAuth);

                app.MapAspNetToEverything();
                iisManager.CommitChanges();

                
            }

            // TODO -- deal with bottle references
        }



    }
}