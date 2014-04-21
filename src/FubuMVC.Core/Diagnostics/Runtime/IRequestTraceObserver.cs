namespace FubuMVC.Core.Diagnostics.Runtime
{
    public interface IRequestTraceObserver
    {
        void Started(RequestLog log);
        void Completed(RequestLog log);
    }
}