using System.Collections.Generic;
using System.Web;

namespace FubuMVC.Core.Http.Owin
{
    public class OwinHttpContext : HttpContextBase
    {
        private readonly AspNetHttpRequestAdapter _request;

        public OwinHttpContext(IDictionary<string, object> environment)
        {
            _request = new AspNetHttpRequestAdapter(environment);
        }

        public override HttpRequestBase Request
        {
            get { return _request; }
        }
    }
}