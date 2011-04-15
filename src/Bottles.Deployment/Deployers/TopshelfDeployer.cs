using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FubuCore;

namespace Bottles.Deployment.Deployers
{
    //assumes its on the same server
    public class TopshelfDeployer : IDeployer<TopshelfService>
    {
        //how am I going to get the TopshelfService?
        private readonly IFileSystem _fileSystem;
        private readonly IBottleRepository _repository;
        private readonly IProcessRunner _runner;
        private readonly IToolRepository _toolRepo;

        public TopshelfDeployer(IFileSystem fileSystem, IBottleRepository repository, IProcessRunner runner, IToolRepository toolRepo)
        {
            _fileSystem = fileSystem;
            _repository = repository;
            _runner = runner;
            _toolRepo = toolRepo;
        }

        public void Deploy()
        {
            var ts = new TopshelfService();

            //copy out TS host
            var pathToBottleHost = _toolRepo.PathTo("bottlehost");
            _fileSystem.Copy(pathToBottleHost, ts.InstallLocation);
            
            //copy out service bottle exploded
            var location = FileSystem.Combine(ts.InstallLocation, "svc");
            _repository.ExplodeTo("bottleToDeploy", location);

            var bottleDest = FileSystem.Combine(ts.InstallLocation, BottleFiles.PackagesFolder);
            ts.Bottles.Each(b =>
                {
                    _repository.CopyTo(b, bottleDest);
                });
            
            var psi = new ProcessStartInfo("Bottles.Host.exe");
            psi.Arguments = "install"; //add args as needed
            psi.WorkingDirectory = "hmm"; //need to tell it where to run from
            psi.UseShellExecute = false; //don't start from cmd.exe
            psi.CreateNoWindow = true; //don't use a window

            _runner.Run(psi);
        }
    }
}