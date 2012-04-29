using FubuCore.Binding;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Http
{
    public class HttpStandInServiceRegistry : ServiceRegistry
    {
        public HttpStandInServiceRegistry()
        {
            SetServiceIfNone<ICurrentHttpRequest, StandInCurrentHttpRequest>();

            SetServiceIfNone<IRequestHeaders, RequestHeaders>();
            SetServiceIfNone<IRequestData>(new RequestData());
        }
    }
}