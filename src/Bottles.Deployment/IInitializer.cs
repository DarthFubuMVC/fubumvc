namespace Bottles.Deployment
{
    public interface IInitializer
    {
        void Initialize(IDirective directive);
    }

    public interface IInitializer<T> : IInitializer where T : IDirective
    {

    }
}