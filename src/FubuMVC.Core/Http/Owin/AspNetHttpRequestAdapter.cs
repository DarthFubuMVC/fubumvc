using System.Collections.Generic;
using System.Web;
using FubuCore;

namespace FubuMVC.Core.Http.Owin
{
    public class AspNetHttpRequestAdapter : HttpRequestBase
    {
        private readonly IDictionary<string, object> _environment;

        public AspNetHttpRequestAdapter(IDictionary<string, object> environment)
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