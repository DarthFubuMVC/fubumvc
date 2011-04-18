using System.Collections.Generic;
using System.Diagnostics;
using FubuCore;

namespace Bottles.Deployment.Deployers
{
    //assumes its on the same server
    public class TopshelfDeployer : IDeployer<TopshelfService>
    {
        private readonly IBottleRepository _repository;
        private readonly IProcessRunner _runner;
        private readonly IToolRepository _toolRepo;

        public TopshelfDeployer(IBottleRepository repository, IProcessRunner runner, IToolRepository toolRepo)
        {
            _repository = repository;
            _runner = runner;
            _toolRepo = toolRepo;
        }

        public void Deploy(IDirective directive)
        {
            var ts = (TopshelfService) directive;

            //copy out TS host
            _toolRepo.CopyTo("bottlehost", ts.InstallLocation);
            
            //copy out service bottle exploded
            var location = FileSystem.Combine(ts.InstallLocation, "svc");
            _repository.ExplodeTo(ts.MainBottle, location);

            var bottleDest = FileSystem.Combine(ts.InstallLocation, BottleFiles.PackagesFolder);
            ts.Bottles.Each(b =>
                {
                    _repository.CopyTo(b, bottleDest);
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