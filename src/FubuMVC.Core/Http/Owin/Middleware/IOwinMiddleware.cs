using System.Collections.Generic;
using System.Threading.Tasks;

namespace FubuMVC.Core.Http.Owin.Middleware
{
    public interface IOwinMiddleware
    {
        Task Invoke(IDictionary<string, object> environment);
    }
}