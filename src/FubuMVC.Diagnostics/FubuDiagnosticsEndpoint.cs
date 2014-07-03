using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.JavascriptRouting;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using HtmlTags;

namespace FubuMVC.Diagnostics
{
    public class FubuDiagnosticsEndpoint
    {
        private readonly JavascriptRouteWriter _routeWriter;
        private readonly DiagnosticJavascriptRoutes _routes;
        private readonly IAssetGraph _assets;
        private readonly IHttpRequest _request;

        public FubuDiagnosticsEndpoint(JavascriptRouteWriter routeWriter, DiagnosticJavascriptRoutes routes, IAssetGraph assets, IHttpRequest request)
        {
            _routeWriter = routeWriter;
            _routes = routes;
            _assets = assets;
            _request = request;
        }

        private IEnumerable<Asset> findAssets(MimeType mimeType)
        {
            return _assets.Assets.Where(x => x.MimeType == mimeType && x.Url.StartsWith("fubu-diagnostics/"));
        } 

        public DashboardModel get__fubu()
        {
            return new DashboardModel
            {
                StyleTags = findAssets(MimeType.Css).Select(x => new StylesheetLinkTag(_request.ToFullUrl(x.Url))).ToArray().ToTagList(),
                ScriptTags = findAssets(MimeType.Javascript).Select(x => new ScriptTag(_ => _request.ToFullUrl(_), x)).ToArray().ToTagList(),
                Router = _routeWriter.WriteJavascriptRoutes("FubuDiagnostics.routes", _routes),
                ReactTags = findAssets(MimeType.MimeTypeByValue("text/jsx")).Select(x => new ScriptTag(_ => _request.ToFullUrl(_), x).Attr("type", "text/jsx")).ToArray().ToTagList()
            };
        }
    }

    public class DiagnosticJavascriptRoutes : JavascriptRouter
    {
        public DiagnosticJavascriptRoutes(BehaviorGraph graph)
        {
            graph.Behaviors.OfType<DiagnosticChain>().Each(Add);
        }
    }

    public class DashboardModel
    {
        public TagList StyleTags { get; set; }
        public TagList ScriptTags { get; set; }
        public TagList ReactTags { get; set; }
        public HtmlTag Router { get; set; }
    }
}