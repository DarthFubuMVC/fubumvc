namespace FubuMVC.Core.Diagnostics.Runtime.Tracing
{
    public interface IRequestLogBuilder
    {
        RequestLog BuildForCurrentRequest();
    }
}