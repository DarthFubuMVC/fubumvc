namespace Bottles.Deployment
{
    public interface IDeployer
    {
        void Deploy();
    }

    public interface IDeployer<T> : IDeployer where T : IDirective
    {
        
    }
}