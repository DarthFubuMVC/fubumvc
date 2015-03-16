using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.JavascriptRouting;
using FubuMVC.Core.Diagnostics.Assets;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics
{

    [Tag("Diagnostics")]
    public class FubuDiagnosticsEndpoint
    {
        private readonly IAssetTagBuilder _tags;
        private readonly IHttpRequest _request;
        private readonly IHttpResponse _response;
        private readonly IDiagnosticAssets _assets;
        private readonly JavascriptRouteWriter _routeWriter;
        private readonly DiagnosticJavascriptRoutes _routes;

        private static readonly string[] _styles = new[] {"bootstrap.min.css", "bootstrap.overrides.css"};
        private static readonly string[] _scripts = new[] {"root.js"};

        public FubuDiagnosticsEndpoint(
            IAssetTagBuilder tags, 
            IHttpRequest request, 
            IHttpResponse response,
            IDiagnosticAssets assets, 
            JavascriptRouteWriter routeWriter, 
            DiagnosticJavascriptRoutes routes)
        {
            _tags = tags;
            _request = request;
            _response = response;
            _assets = assets;
            _routeWriter = routeWriter;
            _routes = routes;
        }

        public HtmlDocument get__fubu()
        {
            var document = new HtmlDocument
            {
                Title = "FubuMVC Diagnostics"
            };

            writeStyles(document);

            var foot = new HtmlTag("foot");
            document.Body.Next = foot;
            writeScripts(foot);




            return document;
        }

        private void writeScripts(HtmlTag foot)
        {
            // Do this regardless
            foot.Append(_assets.For("FubuDiagnostics.js").ToScriptTag());

            var routeData = _routeWriter.WriteJavascriptRoutes("FubuDiagnostics.routes", _routes);
            foot.Append(routeData);

            if (FubuMode.Mode() == "diagnostics")
            {
                var links = _tags.BuildScriptTags(_scripts.Select(x => "fubu-diagnostics/" + x));
                links.Each(x => foot.Append(x));
            }
            else
            {
                _scripts.Each(name =>
                {
                    var file = _assets.For(name);
                    foot.Append(file.ToScriptTag());
                });
            }
        }

        private void writeStyles(HtmlDocument document)
        {
            _styles.Each(name =>
            {
                var file = _assets.For(name);
                document.Head.Append(file.ToStyleTag());
            });
        }

        public void get__fubu_asset_Version_Name(DiagnosticAssetRequest request)
        {
            var file = _assets.For(request.Name);
            file.Write(_response);
        }
    }

    public class DiagnosticAssetRequest
    {
        public string Version { get; set; }
        public string Name { get; set; }
    }

    public class DiagnosticJavascriptRoutes : JavascriptRouter
    {
        public DiagnosticJavascriptRoutes(BehaviorGraph graph)
        {
            graph.Behaviors.OfType<DiagnosticChain>().Where(x => x.Route.AllowedHttpMethods.Any()).Each(Add);
        }
    }
}