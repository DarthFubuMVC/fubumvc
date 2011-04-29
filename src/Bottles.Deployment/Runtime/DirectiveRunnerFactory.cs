using StructureMap;

namespace Bottles.Deployment.Runtime
{
    public class DirectiveRunnerFactory : IDirectiveRunnerFactory
    {
        private readonly IContainer _container;

        public DirectiveRunnerFactory(IContainer container)
        {
            _container = container;
        }

        public IDirectiveRunner Build(IDirective directive)
        {
            return _container.ForObject(directive)
                .GetClosedTypeOf(typeof (DirectiveRunner<>))
                .As<IDirectiveRunner>();
        }
    }
}