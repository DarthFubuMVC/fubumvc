using System.Collections.Generic;
using Bottles.Deployment;
using Bottles.Deployment.Diagnostics;
using Bottles.Deployment.Directives;
using FubuCore;
using Microsoft.Web.Administration;

namespace Bottles.Deployers.Iis
{
    public class IisFubuDeployer : IDeployer<FubuWebsite>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IBottleRepository _bottles;
        private readonly IDeploymentDiagnostics _diagnostics;

        public IisFubuDeployer(IFileSystem fileSystem, IBottleRepository bottles, IDeploymentDiagnostics diagnostics)
        {
            _fileSystem = fileSystem;
            _diagnostics = diagnostics;
            _bottles = bottles;
        }

        //http://www.iis.net/ConfigReference/
        public void Deploy(HostManifest host, IDirective directive)
        {
            _diagnostics.LogDeployment(this, directive);

            var direc = (FubuWebsite) directive;

            //currenly only IIS 7
            using (var iisManager = new ServerManager())
            {
                var pool = iisManager.CreateAppPool(direc.AppPool);
                pool.ManagedRuntimeVersion = "v4.0";
                pool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
                pool.Enable32BitAppOnWin64 = false;

                if (direc.HasCredentials())
                {
                    pool.ProcessModel.UserName = direc.Username;
                    pool.ProcessModel.Password = direc.Password;
                }

                var site = iisManager.CreateSite(direc.WebsiteName, direc.WebsitePhysicalPath, direc.Port);


                var app = site.CreateApplication(direc.VDir, direc.VDirPhysicalPath);
                app.ApplicationPoolName = direc.AppPool;

                //flush the changes so that we can now tweak them.
                iisManager.CommitChanges();



                MoveWebContentOut(direc);





                app.DirectoryBrowsing(direc.DirectoryBrowsing);

                //app.AnonAuthentication(direc.AnonAuth);
                //app.BasicAuthentication(direc.BasicAuth);
                //app.WindowsAuthentication(direc.WindowsAuth);

                app.MapAspNetToEverything();
                iisManager.CommitChanges();
            }

           
        }

        private void MoveWebContentOut(FubuWebsite direc)
        {
            _bottles.ExplodeTo(direc.HostBottle, direc.VDirPhysicalPath);

            var webContent = FileSystem.Combine(direc.VDirPhysicalPath, "WebContent");
            _fileSystem.MoveFiles(webContent, direc.VDirPhysicalPath);

            _fileSystem.DeleteDirectory(webContent);
        }
    }
}