using System.Collections.Generic;
using Bottles.Deployment.Diagnostics;

namespace Bottles.Deployment.Runtime
{
    public class DirectiveRunner : IDirectiveRunner
    {
        private readonly ILogger _logger;
        private readonly ICommandFactory _factory;

        public DirectiveRunner(ILogger logger, ICommandFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }

        public void Deploy(IEnumerable<HostManifest> hosts)
        {
            foreach (var hostManifest in hosts)
            {
                _logger.LogHost(hostManifest, h =>
                    {
                        foreach (var directive in h.AllDirectives)
                        {
                            _factory.InitializersFor(directive)
                                .Initialize();
                            
                            _factory.DeployersFor(directive)
                                .Deploy();

                            _factory.FinalizersFor(directive)
                                .Finish();
                        }
                    });

            }
            
        }
    }
}