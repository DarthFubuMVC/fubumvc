using System;
using System.Web;
using Gate.Helpers;

namespace FubuMVC.OwinHost
{
    public class GateHttpContext : HttpContextBase
    {
        private readonly GateHttpRequest _request;

        public GateHttpContext(Request request)
        {
            _request = new GateHttpRequest(request);
        }

        public override HttpRequestBase Request
        {
            get { return _request; }
        }
    }

    public class GateHttpRequest : HttpRequestBase
    {
        private readonly Request _request;

        public GateHttpRequest(Request request)
        {
            _request = request;
        }

        public override string PathInfo
        {
            get { return _request.Path.TrimStart('/'); }
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get { return "~/"; }
        }
    }
}