using System.Web;

namespace FubuMVC.OwinHost
{
    public class GateHttpContext : HttpContextBase
    {
        private readonly GateHttpRequest _request;

        public GateHttpContext(string path, string method)
        {
            _request = new GateHttpRequest(path, method);
        }

        public override HttpRequestBase Request
        {
            get { return _request; }
        }
    }

    public class GateHttpRequest : HttpRequestBase
    {
        private readonly string _method;
        private readonly string _path;

        public GateHttpRequest(string path, string method)
        {
            _path = path;
            _method = method;
        }

        public override string PathInfo
        {
            get { return _path.TrimStart('/'); }
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get { return "~/"; }
        }

        public override string HttpMethod
        {
            get { return _method; }
        }
    }
}