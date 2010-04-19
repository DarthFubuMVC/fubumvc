using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.UI.Forms;
using FubuMVC.UI.Tags;

namespace FubuMVC.UI.Configuration
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
            graph.Services.SetServiceIfNone<ILabelAndFieldLayout, DefinitionListLabelAndField>();
        }
    }
}