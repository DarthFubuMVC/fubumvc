using System.Collections.Generic;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.UI.Diagnostics;
using FubuMVC.Core.UI.Security;
using FubuMVC.Core.UI.Tags;

namespace FubuMVC.Core.UI.Configuration
{
    public class HtmlConventionCompiler : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var library = new TagProfileLibrary();

            graph.Services.FindAllValues<HtmlConventionRegistry>()
                .Each(library.ImportRegistry);

            library.ImportRegistry(new DefaultHtmlConventions());

            library.Seal();

            graph.Services.ClearAll<HtmlConventionRegistry>();
            graph.Services.ReplaceService(library);
            graph.Services.SetServiceIfNone(typeof(ITagGenerator<>), typeof(TagGenerator<>));
            graph.Services.SetServiceIfNone<IElementNamingConvention, DefaultElementNamingConvention>();
            graph.Services.SetServiceIfNone<IFieldAccessService, FieldAccessService>();

            if (graph.IsDiagnosticsEnabled())
            {
                graph.Services.SetServiceIfNone<IFieldAccessRightsExecutor, RecordingFieldAccessRightsExecutor>();
            }
            else
            {
                graph.Services.SetServiceIfNone<IFieldAccessRightsExecutor, FieldAccessRightsExecutor>();
            }
        }
    }
}