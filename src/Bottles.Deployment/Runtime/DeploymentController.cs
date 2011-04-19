using System.Collections.Generic;
using Bottles.Deployment.Parsing;

namespace Bottles.Deployment.Runtime
{
    public interface IDeploymentController
    {
        
    }

    public interface IDirectiveRunner
    {
        void Deploy(IEnumerable<HostManifest> hosts);
    }




    public class DeploymentController : IDeploymentController
    {
        private readonly IProfileReader _reader;
        private readonly IDirectiveRunner _runner;

        public DeploymentController(IProfileReader reader, IDirectiveRunner runner)
        {
            _reader = reader;
            _runner = runner;
        }


    }

    public class DeployerFactory
    {
        
    }
}