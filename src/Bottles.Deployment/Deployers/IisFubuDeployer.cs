using System.Collections.Generic;
using FubuCore;
using Microsoft.Web.Administration;

namespace Bottles.Deployment.Deployers
{
    public class IisFubuDeployer : IDeployer<IisFubuWebsite>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IBottleRepository _bottles;

        public IisFubuDeployer(IFileSystem fileSystem, IBottleRepository bottles)
        {
            _fileSystem = fileSystem;
            _bottles = bottles;
        }

        public void Deploy(IDirective directive)
        {
            var direc = (IisFubuWebsite) directive;

            _fileSystem.CreateDirectory(direc.VDirPhysicalPath);

            //REVIEW: currently this is grouped per site touched - now its one by one
            var appOfflineFile = FileSystem.Combine(direc.VDirPhysicalPath, "app_offline.htm");

            _fileSystem.WriteStringToFile(appOfflineFile, "&lt;html&gt;&lt;body&gt;Application is being rebuilt&lt;/body&gt;&lt;/html&gt;");
            

            //currenly only IIS 7
            using(var iisManager = ServerManager.OpenRemote("."))
            {
                var site = iisManager.CreateSite(direc.WebsiteName, direc.WebsitePhysicalPath, direc.Port);

                iisManager.CreateAppPool(direc.AppPool, pool =>
                    {
                        pool.ManagedRuntimeVersion = "v4.0";
                        pool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
                        pool.Enable32BitAppOnWin64 = false;

                        if(direc.HasCredentials())
                        {
                            pool.ProcessModel.UserName = direc.Username;
                            pool.ProcessModel.Password = direc.Password;
                        }
                        
                    });

                var app = site.CreateApplication(direc.VDir, direc.VDirPhysicalPath);
                
                app.ApplicationPoolName = direc.AppPool;
                app.DirectoryBrowsing(direc.DirectoryBrowsing);

                app.AnonAuthentication(direc.AnonAuth);
                app.BasicAuthentication(direc.BasicAuth);
                app.WindowsAuthentication(direc.WindowsAuth);
                
                app.MapAspNetToEverything();

                //icacls

                iisManager.CommitChanges();
            }


            //host bottle
            _bottles.ExplodeTo(direc.HostBottle, direc.VDirPhysicalPath);

            //REVIEW: A - not going to work
            var bottleDest = FileSystem.Combine(direc.VDirPhysicalPath, "packages");
            direc.Bottles.Each(b =>
            {
                _bottles.CopyTo(b, bottleDest);
            });
            

            _fileSystem.DeleteFile(appOfflineFile);
        }
    }
}