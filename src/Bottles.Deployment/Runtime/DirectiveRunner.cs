using System.Collections.Generic;
using Bottles.Deployment.Diagnostics;

namespace Bottles.Deployment.Runtime
{
    public interface IDirectiveRunner
    {
        void Attach(HostManifest host, IDirective directive);
        void InitializeDeployment();
        void Deploy();
        void FinalizeDeployment();
    }

    public class DirectiveRunner<T> : IDirectiveRunner where T : IDirective
    {
        private readonly IDeploymentDiagnostics _diagnostics;
        private readonly IEnumerable<IDeployer<T>> _deployers;
        private readonly IEnumerable<IInitializer<T>> _initializers;
        private readonly IEnumerable<IFinalizer<T>> _finalizers;
        private HostManifest _host;
        private T _directive;

        public DirectiveRunner(IDeploymentDiagnostics diagnostics, IEnumerable<IDeployer<T>> deployers, IEnumerable<IInitializer<T>> initializers, IEnumerable<IFinalizer<T>> finalizers)
        {
            _diagnostics = diagnostics;
            _deployers = deployers;
            _initializers = initializers;
            _finalizers = finalizers;
        }

        public void Attach(HostManifest host, IDirective directive)
        {
            _host = host;
            _directive = (T)directive;

            _diagnostics.LogDirective(host, directive);
        }

        public void InitializeDeployment()
        {
            runActions(_initializers);
        }

        public void Deploy()
        {
            runActions(_deployers);
        }

        public void FinalizeDeployment()
        {
            runActions(_finalizers);
        }

        public IEnumerable<IDeployer<T>> Deployers
        {
            get { return _deployers; }
        }

        public IEnumerable<IInitializer<T>> Initializers
        {
            get { return _initializers; }
        }

        public IEnumerable<IFinalizer<T>> Finalizers
        {
            get { return _finalizers; }
        }

        private void runActions(IEnumerable<IDeploymentAction<T>> actions)
        {
            actions.Each(x =>
            {
                var log = _diagnostics.LogAction(_host, _directive, x);
                log.Execute(() => x.Execute(_directive, _host, log));
            });
        }
    }
}