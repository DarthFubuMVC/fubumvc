using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.JavascriptRouting;
using HtmlTags;

namespace FubuMVC.Diagnostics
{
    public class FubuDiagnosticsEndpoint
    {
        private readonly JavascriptRouteWriter _routeWriter;
        private readonly DiagnosticsSettings _settings;
        private readonly IAssetTagBuilder _tagBuilder;

        public FubuDiagnosticsEndpoint(JavascriptRouteWriter routeWriter, DiagnosticsSettings settings, IAssetTagBuilder tagBuilder)
        {
            _routeWriter = routeWriter;
            _settings = settings;
            _tagBuilder = tagBuilder;
        }

        public DashboardModel get__fubu()
        {
            var scriptTags = _tagBuilder.BuildScriptTags(_settings.Scripts()).ToArray();
            var reactTags = _tagBuilder.BuildScriptTags(_settings.ReactFiles()).ToArray();
            reactTags.Each(x => x.Attr("type", "text/jsx"));

            return new DashboardModel
            {
                StyleTags = _tagBuilder.BuildStylesheetTags(_settings.Stylesheets()).ToTagList(),
                ScriptTags = scriptTags.ToTagList(),
                Router = _routeWriter.WriteJavascriptRoutes("FubuDiagnostics.routes", _settings.ToJavascriptRoutes()),
                ReactTags = reactTags.ToTagList()
            };
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