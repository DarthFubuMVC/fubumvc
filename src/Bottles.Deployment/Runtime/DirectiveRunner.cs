using System.Collections.Generic;
using Bottles.Deployment.Diagnostics;

namespace Bottles.Deployment.Runtime
{
    public class DirectiveRunner : IDirectiveRunner
    {
        private readonly IDeploymentDiagnostics _diagnostics;
        private readonly IDirectiveCoordinator _coordinator;

        public DirectiveRunner(IDeploymentDiagnostics diagnostics, IDirectiveCoordinator coordinator)
        {
            _diagnostics = diagnostics;
            _coordinator = coordinator;
        }

        public void Deploy(IEnumerable<HostManifest> hosts)
        {
            //assuming hosts are sorted
            _diagnostics.LogExecution(_coordinator, "Initializing the hosts.", () =>
            {
                _coordinator.Initialize(hosts);
            });


            _diagnostics.LogExecution(_coordinator,"Deploying to the hosts", () =>
            {
                _coordinator.Deploy(hosts);
            });

            //reverse sorting order?
            _diagnostics.LogExecution(_coordinator,"Finalizing the hosts" ,() =>
            {
                _coordinator.Finish(hosts);
            });

        }
    }
}