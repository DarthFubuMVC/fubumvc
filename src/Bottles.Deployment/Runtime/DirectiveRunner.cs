using System;
using System.Collections.Generic;
using Bottles.Deployment.Diagnostics;

namespace Bottles.Deployment.Runtime
{
    public class DirectiveRunner : IDirectiveRunner
    {
        private readonly ILogger _logger;
        private readonly IDirectiveCoordinator _factory;

        public DirectiveRunner(ILogger logger, IDirectiveCoordinator factory)
        {
            _logger = logger;
            _factory = factory;
        }

        public void Deploy(IEnumerable<HostManifest> hosts)
        {
            //assuming hosts are sorted
            _logger.Log("runner:init", () =>
            {
                _factory.Initialize(hosts);
            });


            _logger.Log("runner:deploy", () =>
            {
                _factory.Deploy(hosts);
            });

            //reverse sorting order?
            _logger.Log("runner:finish", () =>
            {
                _factory.Finish(hosts);
            });

        }
    }
}