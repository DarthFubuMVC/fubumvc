namespace Bottles.Deployment.Runtime
{
    public interface IDeploymentController
    {
        void Deploy(DeploymentOptions options);
    }
}