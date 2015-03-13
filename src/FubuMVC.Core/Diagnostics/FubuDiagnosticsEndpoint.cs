using System.Collections.Generic;
using System.Linq;
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
        private readonly IHttpRequest _request;
        private readonly IHttpResponse _response;
        private readonly IDiagnosticAssets _assets;

        private static readonly string[] _styles = new[] {"bootstrap.min.css", "master.css", "bootstrap.overrides.css"};

        public FubuDiagnosticsEndpoint(IHttpRequest request, IHttpResponse response, IDiagnosticAssets assets)
        {
            _request = request;
            _response = response;
            _assets = assets;
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


            return document;
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