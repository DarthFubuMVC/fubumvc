using System.Collections.Generic;
using System.Web;

namespace FubuMVC.Core.Assets.Http
{
    public class AssetHttpHandler : IHttpHandler
    {
        private readonly IEnumerable<string> _routeValues;
        private readonly IContentWriter _writer;

        public AssetHttpHandler(IContentWriter writer, IEnumerable<string> routeValues)
        {
            _writer = writer;
            _routeValues = routeValues;
        }

        public void ProcessRequest(HttpContext context)
        {
            _writer.WriteContent(_routeValues);
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}