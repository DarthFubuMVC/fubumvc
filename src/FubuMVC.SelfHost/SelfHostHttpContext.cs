using System.Net.Http;
using System.Web;

namespace FubuMVC.SelfHost
{
    public class SelfHostHttpContext : HttpContextBase
    {
        private readonly SelfHostHttpRequest _request;

        public SelfHostHttpContext(HttpRequestMessage request)
        {
            _request = new SelfHostHttpRequest(request);
        }

        public override HttpRequestBase Request
        {
            get { return _request; }
        }
    }
}