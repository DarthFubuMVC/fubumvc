namespace Bottles.Deployment.Runtime
{
    public interface ICommandFactory
    {
        IDeployerSet DeployersFor(IDirective directive);
        IInitializerSet InitializersFor(IDirective directive);
        IFinalizerSet FinalizersFor(IDirective directive);
    }
}