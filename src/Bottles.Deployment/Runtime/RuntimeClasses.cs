using System;
using System.Collections.Generic;
using Bottles.Deployment.Diagnostics;
using Bottles.Diagnostics;
using StructureMap;

namespace Bottles.Deployment.Runtime
{
    public interface IDeploymentAction<T> where T : IDirective
    {
        void Execute(T directive, HostManifest host, IPackageLog log);
    }

    public interface IInitializer<T> : IDeploymentAction<T> where T : IDirective
    {

    }


    public interface IFinalizer<T> : IDeploymentAction<T> where T : IDirective
    {

    }

    public interface IDeployer<T> : IDeploymentAction<T> where T : IDirective
    {

    }


    public class DirectiveRunner<T> where T : IDirective
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

        private void runActions(IEnumerable<IDeploymentAction<T>> actions)
        {
            actions.Each(x =>
            {
                var log = _diagnostics.LogAction(_host, _directive, x);
                log.Execute(() => x.Execute(_directive, _host, log));
            });
        }
    }

    public interface ICommandFactory
    {

    }

    public class CommandFactory : ICommandFactory
    {
        private readonly IContainer _container;

        public CommandFactory(IContainer container)
        {
            _container = container;
        }

        //public IDeploymentActionSet InitializersFor(IDirective directive)
        //{
        //    return _container
        //        .ForObject(directive)
        //        .GetClosedTypeOf(typeof (InitializerSet<>))
        //        .As<IDeploymentActionSet>();
        //}

    }


}