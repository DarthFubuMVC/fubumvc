using Bottles.Deployment.Directives;
using Microsoft.Web.Administration;

namespace Bottles.Deployers.Iis
{
    public class IisWebsiteCreator
    {
        public void Create(Website website)
        {
            //currenly only IIS 7
            using (var iisManager = new ServerManager())
            {
                var pool = iisManager.CreateAppPool(website.AppPool);
                pool.ManagedRuntimeVersion = "v4.0";
                pool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
                pool.Enable32BitAppOnWin64 = false;

                if (website.HasCredentials())
                {
                    pool.ProcessModel.UserName = website.Username;
                    pool.ProcessModel.Password = website.Password;
                }

                var site = iisManager.CreateSite(website.WebsiteName, website.WebsitePhysicalPath, website.Port);


                var app = site.CreateApplication(website.VDir, website.VDirPhysicalPath);
                app.ApplicationPoolName = website.AppPool;

                //flush the changes so that we can now tweak them.
                iisManager.CommitChanges();

                app.DirectoryBrowsing(website.DirectoryBrowsing);

                app.AnonAuthentication(website.AnonAuth);
                app.BasicAuthentication(website.BasicAuth);
                app.WindowsAuthentication(website.WindowsAuth);

                app.MapAspNetToEverything();
                iisManager.CommitChanges();
            }
        }
    }
}