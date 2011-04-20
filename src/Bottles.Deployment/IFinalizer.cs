namespace Bottles.Deployment
{
    public interface IFinalizer
    {
        void Finish(IDirective directive);
    }


    public interface IFinalizer<T> : IFinalizer where T : IDirective
    {

    }
}