namespace Bottles.Deployment.Runtime
{
    public interface ICommandFactory
    {
        IDeploymentActionSet InitializersFor(IDirective directive);
        IDeploymentActionSet DeployersFor(IDirective directive);
        IDeploymentActionSet FinalizersFor(IDirective directive);
    }
}