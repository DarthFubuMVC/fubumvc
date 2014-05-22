namespace Fubu.Running
{
    public interface IApplicationObserver
    {
        // Refresh the browser
        void RefreshContent();

        // Tear down and reload the entire AppDomain
        void RecycleAppDomain();

        // Restart the FubuMVC application
        // without restarting the 
        // AppDomain
        void RecycleApplication();
    }
}