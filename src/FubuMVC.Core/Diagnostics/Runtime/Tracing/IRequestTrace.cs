namespace FubuMVC.Core.Diagnostics.Runtime.Tracing
{
    public interface IRequestTrace
    {
        void Start();
        void MarkFinished();
        void Log(object message);
        void MarkAsFailedRequest();
        RequestLog Current { get; set; }
    }
}