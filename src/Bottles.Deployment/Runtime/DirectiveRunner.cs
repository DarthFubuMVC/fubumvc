using System.Collections.Generic;
using Bottles.Deployment.Diagnostics;

namespace Bottles.Deployment.Runtime
{
    public class DirectiveRunner : IDirectiveRunner
    {
        private readonly IDeploymentDiagnostics _diagnostics;
        private readonly IDirectiveCoordinator _factory;

        public DirectiveRunner(IDeploymentDiagnostics diagnostics, IDirectiveCoordinator factory)
        {
            _diagnostics = diagnostics;
            _factory = factory;
        }

        public void Deploy(IEnumerable<HostManifest> hosts)
        {
            //assuming hosts are sorted
            _diagnostics.Log("runner:init", () =>
            {
                _factory.Initialize(hosts);
            });


            _diagnostics.Log("runner:deploy", () =>
            {
                _factory.Deploy(hosts);
            });

            //reverse sorting order?
            _diagnostics.Log("runner:finish", () =>
            {
                _factory.Finish(hosts);
            });

        }
    }
}