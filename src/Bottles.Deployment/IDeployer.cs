namespace Bottles.Deployment
{
    public interface IDeployer
    {
        void Deploy(HostManifest manifest, IDirective directive);
    }

    public interface IDeployer<T> : IDeployer where T : IDirective
    {
        
    }
}