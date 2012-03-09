using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Http
{
    public class HttpStandInServiceRegistry : ServiceRegistry
    {
        public HttpStandInServiceRegistry()
        {
            SetServiceIfNone<ICurrentHttpRequest, StandInCurrentHttpRequest>();

            AddService<ICurrentHttpRequest, StandInCurrentHttpRequest>();
            SetServiceIfNone<IRequestHeaders, RequestHeaders>();
        }
    }
}