using System;
using System.Collections.Generic;
using Bottles.Deployment.Diagnostics;

namespace Bottles.Deployment.Runtime
{
    public class DirectiveCoordinator : IDirectiveCoordinator
    {
        private readonly ICommandFactory _factory;
        private IDeploymentDiagnostics _diagnostics;

        public DirectiveCoordinator(ICommandFactory factory, IDeploymentDiagnostics diagnostics)
        {
            _factory = factory;
            _diagnostics = diagnostics;
        }

        public void Initialize(IEnumerable<HostManifest> hosts)
        {
            applyProcessToEachDirective(hosts, _factory.InitializersFor);
        }

        public void Deploy(IEnumerable<HostManifest> hosts)
        {
            applyProcessToEachDirective(hosts, _factory.DeployersFor);
        }

        public void Finish(IEnumerable<HostManifest> hosts)
        {
            applyProcessToEachDirective(hosts, _factory.FinalizersFor);
        }

        private void applyProcessToEachDirective(IEnumerable<HostManifest> hosts, Func<IDirective, IDeploymentActionSet> action)
        {
            foreach (var host in hosts)
            {
                _diagnostics.LogHost(host);

                foreach (var directive in host.AllDirectives)
                {
                    _diagnostics.LogDirective(directive, host);

                    var set = action(directive);
                    set.Process(host, directive);
                }
            }
        }
    }
}