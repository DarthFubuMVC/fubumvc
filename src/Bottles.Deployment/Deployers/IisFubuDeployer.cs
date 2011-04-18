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

            _fileSystem.CreateDirectory(direc.AppDirectory);

            //host bottle
            _bottles.ExplodeTo(direc.MainBottle, direc.AppDirectory);

            //REVIEW: A - not going to work
            var bottleDest = FileSystem.Combine(direc.AppDirectory, BottleFiles.PackagesFolder);
            direc.Bottles.Each(b =>
                {
                    _bottles.CopyTo(b, bottleDest);
                });


            //currenly only IIS 7
            using(var iisManager = ServerManager.OpenRemote("."))
            {
                var site = iisManager.CreateSite(direc.WebsiteName, direc.WebsitePath, direc.Port);
                var app = site.CreateApplication(direc.VDir, direc.AppDirectory);

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

                app.ApplicationPoolName = direc.AppPool;


                iisManager.CommitChanges();
            }
                       
        }
    }
}