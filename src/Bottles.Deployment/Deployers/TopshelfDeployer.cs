using System.Collections.Generic;
using System.Diagnostics;
using FubuCore;

namespace Bottles.Deployment.Deployers
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

        public void Deploy(IDirective directive)
        {
            var ts = (TopshelfService) directive;

            //copy out TS host
            _bottles.ExplodeTo("bottlehost", ts.InstallLocation);

            //copy out service bottle exploded
            var location = FileSystem.Combine(ts.InstallLocation, "svc");
            _bottles.ExplodeTo(ts.HostBottle, location);

            var bottleDest = FileSystem.Combine(ts.InstallLocation, "packages");
            ts.Bottles.Each(b =>
                {
                    _bottles.CopyTo(b, bottleDest);
                });
            
            var psi = new ProcessStartInfo("Bottles.Host.exe")
            {
                Arguments = "install",
                WorkingDirectory = ts.InstallLocation
            };

            _runner.Run(psi);
        }
    }
}