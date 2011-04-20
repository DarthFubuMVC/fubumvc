namespace Bottles.Deployment.Runtime
{
    public interface IInitializerSet
    {
        void Initialize(IDirective directive);
    }
}