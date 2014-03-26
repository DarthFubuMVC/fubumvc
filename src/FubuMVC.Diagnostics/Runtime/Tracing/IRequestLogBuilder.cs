namespace FubuMVC.Diagnostics.Runtime.Tracing
{
    public interface IRequestLogBuilder
    {
        RequestLog BuildForCurrentRequest();
    }
}