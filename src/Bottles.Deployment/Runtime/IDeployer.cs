namespace Bottles.Deployment.Runtime
{
    public interface IDeployer<T> : IDeploymentAction<T> where T : IDirective
    {

    }
}