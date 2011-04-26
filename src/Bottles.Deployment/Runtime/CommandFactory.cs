using StructureMap;

namespace Bottles.Deployment.Runtime
{
    public class CommandFactory : ICommandFactory
    {
        private readonly IContainer _container;

        public CommandFactory(IContainer container)
        {
            _container = container;
        }

        public IDeploymentActionSet InitializersFor(IDirective directive)
        {
            return _container
                .ForObject(directive)
                .GetClosedTypeOf(typeof (InitializerSet<>))
                .As<IDeploymentActionSet>();
        }

        public IDeploymentActionSet DeployersFor(IDirective directive)
        {
            return _container
                .ForObject(directive)
                .GetClosedTypeOf(typeof (DeployerSet<>))
                .As<IDeploymentActionSet>();
        }

        public IDeploymentActionSet FinalizersFor(IDirective directive)
        {
            return _container
                .ForObject(directive)
                .GetClosedTypeOf(typeof (FinalizerSet<>))
                .As<IDeploymentActionSet>();
        }
    }
}