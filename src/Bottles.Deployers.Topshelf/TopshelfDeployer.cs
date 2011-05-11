using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Bottles.Deployment;
using Bottles.Deployment.Directives;
using Bottles.Deployment.Runtime;
using Bottles.Deployment.Runtime.Content;
using Bottles.Diagnostics;
using FubuCore;

namespace Bottles.Deployers.Topshelf
{
    //assumes its on the same server
    public class TopshelfDeployer : IDeployer<TopshelfService>
    {
        private readonly IBottleRepository _bottles;
        private readonly IProcessRunner _runner;

        public TopshelfDeployer(IBottleRepository bottles, IProcessRunner runner)
        {
            _bottles = bottles;
            _runner = runner;
        }

        public void Execute(TopshelfService directive, HostManifest host, IPackageLog log)
        {
            //copy out TS host
            _bottles.ExplodeTo("bottlehost", directive.InstallLocation);

            //copy out service bottle exploded
            var location = FileSystem.Combine(directive.InstallLocation, "svc");
            _bottles.ExplodeTo(directive.HostBottle, location);

            var args = buildInstallArgs(directive);
            var psi = new ProcessStartInfo("Bottles.Host.exe")
            {
                Arguments = args,
                WorkingDirectory = directive.InstallLocation
            };

            _runner.Run(psi);
        }

        private static string buildInstallArgs(TopshelfService directive)
        {
            var sb = new StringBuilder();
            sb.Append("install");

            directive.DisplayName.IsNotEmpty(s => sb.AppendFormat(" -displayname:{0}", s));
            directive.Description.IsNotEmpty(s => sb.AppendFormat(" -description:{0}", s));
            directive.ServiceName.IsNotEmpty(s => sb.AppendFormat(" -servicename:{0}", s));
            
            
            directive.Username.IsNotEmpty(s => sb.AppendFormat(" -username:{0}", s));
            directive.Password.IsNotEmpty(s => sb.AppendFormat(" -password:{0}", s));

            
            return sb.ToString();
        }


    }
    
}