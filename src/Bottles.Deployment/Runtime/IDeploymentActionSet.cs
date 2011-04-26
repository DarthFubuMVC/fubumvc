namespace Bottles.Deployment.Runtime
{
    public interface IDeploymentActionSet
    {
        void Process(HostManifest hostManifest, IDirective directive);
    }
}