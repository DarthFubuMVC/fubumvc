namespace Bottles.Deployment
{
    public interface IDeployer
    {
        void Deploy(IDirective directive);
    }

    public interface IDeployer<T> : IDeployer where T : IDirective
    {
        
    }
}