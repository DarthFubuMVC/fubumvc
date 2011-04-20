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

        public IDeployerSet DeployersFor(IDirective directive)
        {
            return _container
                .ForObject(directive)
                .GetClosedTypeOf(typeof (DeployerSet<>))
                .As<IDeployerSet>();
        }

        public IInitializerSet InitializersFor(IDirective directive)
        {
            return _container
                .ForObject(directive)
                .GetClosedTypeOf(typeof (InitializerSet<>))
                .As<IInitializerSet>();
        }

        public IFinalizerSet FinalizersFor(IDirective directive)
        {
            return _container
                .ForObject(directive)
                .GetClosedTypeOf(typeof (FinalizerSet<>))
                .As<IFinalizerSet>();
        }
    }
}