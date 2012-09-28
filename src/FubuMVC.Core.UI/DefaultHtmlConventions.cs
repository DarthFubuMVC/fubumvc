using FubuHtml.Elements.Builders;

namespace FubuHtml
{
    public class DefaultHtmlConventions : HtmlConventionRegistry
    {
        public DefaultHtmlConventions()
        {
            Editors.Builder<CheckboxBuilder>();
            Editors.Builder<TextboxBuilder>();

            Editors.Modifier<AddNameModifier>();

            Displays.Builder<SpanDisplayBuilder>();

            Labels.Builder<DefaultLabelBuilder>();
        }
    }
}