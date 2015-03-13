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

        public FubuDiagnosticsEndpoint(IHttpRequest request, IHttpResponse response, IDiagnosticAssets assets)
        {
            _request = request;
            _response = response;
            _assets = assets;
        }

        public HtmlDocument get__fubu()
        {
            return new HtmlDocument
            {
                Title = "FubuMVC Diagnostics"
            };
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