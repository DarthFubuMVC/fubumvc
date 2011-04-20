namespace Bottles.Deployment.Runtime
{
    public interface IDeploymentActionSet
    {
        void DeployWith(IDirective directive);
    }
}