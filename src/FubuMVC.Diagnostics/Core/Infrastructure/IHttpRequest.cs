using FubuCore;

namespace FubuMVC.Diagnostics.Core.Infrastructure
{
    [MarkedForTermination("Need to get rid of this")]
    public interface IHttpRequest
    {
        string CurrentUrl();
    }
}