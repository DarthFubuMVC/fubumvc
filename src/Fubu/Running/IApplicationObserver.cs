namespace Fubu.Running
{
    public interface IApplicationObserver
    {
        void RefreshContent();
        void RecycleAppDomain();
        void RecycleApplication();
    }
}