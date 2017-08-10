using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Web;

namespace FubuMVC.Core.Http.Owin
{
    public class OwinHttpContext : HttpContextBase
    {
        private readonly AspNetHttpRequestAdapter _request;
        private readonly HttpListenerContext _listenerContext;

        public OwinHttpContext(IDictionary<string, object> environment)
        {
            _request = new AspNetHttpRequestAdapter(environment);
            if (environment.ContainsKey("System.Net.HttpListenerContext"))
            {
                _listenerContext = environment["System.Net.HttpListenerContext"] as HttpListenerContext;
            }
        }

        public override HttpRequestBase Request
        {
            get { return _request; }
        }

        public override IPrincipal User
        {
            get { return _listenerContext?.User; }
        }


    }
}
