using FubuMVC.Core.UI.Elements.Builders;

namespace FubuMVC.Core.UI
{
    public class DefaultHtmlConventions : HtmlConventionRegistry
    {
        public DefaultHtmlConventions()
        {
            Editors.BuilderPolicy<CheckboxBuilder>();

            Editors.Always.BuildBy<TextboxBuilder>();

            Editors.Modifier<AddNameModifier>();

            Displays.Always.BuildBy<SpanDisplayBuilder>();

            Labels.Always.BuildBy<DefaultLabelBuilder>();

            Templates.Displays.Always.BuildBy<TemplateSpanBuilder>();
            Templates.Editors.Always.BuildBy<TemplateTextboxBuilder>();

            Templates.Displays.Always.ModifyWith<DataFldModifier>();
            Templates.Editors.Always.ModifyWith<DataFldModifier>();
        }
    }
}