using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using FubuCore;

namespace FubuMVC.OwinHost
{
    public class OwinHttpRequest : HttpRequestBase
    {
        private readonly IDictionary<string, object> _environment;

        public OwinHttpRequest(IDictionary<string, object> environment)
        {
            _environment = environment;
            
        }

        public override string PathInfo
        {
            get { return _environment.Get<string>(OwinConstants.RequestPathKey).Substring(1); }
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get { return "~/"; }
        }

        public override string HttpMethod
        {
            get { return _environment.Get<string>(OwinConstants.RequestMethodKey); }
        }
    }
}