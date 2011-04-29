using System;
using System.Collections.Generic;
using FubuCore.Binding;
using StructureMap;
using System.Linq;

namespace Bottles.Deployment.Runtime
{
    public class DirectiveRunnerFactory : IDirectiveRunnerFactory
    {
        private readonly IContainer _container;
        private readonly IDirectiveTypeRegistry _types;

        public DirectiveRunnerFactory(IContainer container, IDirectiveTypeRegistry types)
        {
            _container = container;
            _types = types;
        }

        // Take this out of the public interface
        public IDirectiveRunner Build(IDirective directive)
        {
            return _container.ForObject(directive)
                .GetClosedTypeOf(typeof (DirectiveRunner<>))
                .As<IDirectiveRunner>();
        }

        public IEnumerable<IDirectiveRunner> BuildRunners(IEnumerable<HostManifest> hosts)
        {
            return hosts.SelectMany(BuildRunnersFor);
        }

        public IEnumerable<IDirectiveRunner> BuildRunnersFor(HostManifest host)
        {
            foreach (var directive in host.BuildDirectives(_types))
            {
                var runner = Build(directive);
                runner.Attach(host, directive);

                yield return runner;
            }
        }
    }
}