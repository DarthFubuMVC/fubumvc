using System.Collections.Generic;
using System.Linq;

namespace Bottles.Deployment.Runtime
{
    public class DirectiveCoordinator : IDirectiveCoordinator
    {
        private readonly ICommandFactory _factory;

        public DirectiveCoordinator(ICommandFactory factory)
        {
            _factory = factory;
        }

        public void Initialize(IEnumerable<HostManifest> hosts)
        {
            var directives = from h in hosts
                             from d in h.AllDirectives
                             select d;

            //sorting?

            directives.Each(d =>
                            {
                                var set = _factory.InitializersFor(d);
                                set.DeployWith(d);
                            });
        }

        public void Deploy(IEnumerable<HostManifest> hosts)
        {
            var directives = from h in hosts
                             from d in h.AllDirectives
                             select d;

            //sorting?

            directives.Each(d =>
                            {
                                var set = _factory.DeployersFor(d);
                                set.DeployWith(d);
                            });
        }

        public void Finish(IEnumerable<HostManifest> hosts)
        {
            var directives = from h in hosts
                             from d in h.AllDirectives
                             select d;

            //sorting?

            directives.Each(d =>
                            {
                                var set = _factory.FinalizersFor(d);
                                set.DeployWith(d);
                            });
        }
    }
}