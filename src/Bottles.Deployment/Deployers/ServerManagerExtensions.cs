using System;
using System.Linq;
using Microsoft.Web.Administration;

namespace Bottles.Deployment.Deployers
{
    public static class ServerManagerExtensions
    {
        public static Site CreateSite(this ServerManager iisManager, string name, string directory, int port)
        {
            if (iisManager.Sites.Any(s => s.Name.Equals(name)))
            {
                return iisManager.Sites[name];
            }

            return iisManager.Sites.Add(name, directory, port);
        }

        public static Application CreateApplication(this Site site, string vdir, string physicalPath)
        {
            //needs to start with this
            if (vdir[0] != '/') vdir = '/' + vdir;

            if (site.Applications.Any(app => app.Path.Equals(vdir)))
            {
                return site.Applications[vdir];
            }

            return site.Applications.Add(vdir, physicalPath);

        }

        public static void CreateAppPool(this ServerManager iisManager, string name, Action<ApplicationPool> cfg)
        {
            if (!iisManager.ApplicationPools.Any(p => p.Name.Equals(name)))
            {
                iisManager.ApplicationPools.Add(name);
            }

            var pool = iisManager.ApplicationPools[name];
            cfg(pool);
        }


    }
}