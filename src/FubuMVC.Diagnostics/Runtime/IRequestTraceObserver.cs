namespace FubuMVC.Diagnostics.Runtime
{
    public interface IRequestTraceObserver
    {
        void Started(RequestLog log);
        void Completed(RequestLog log);
    }
}