using System.ComponentModel;
using HtmlTags;

namespace FubuMVC.Core.UI.Elements.Builders
{
    [Description("Builds a span element for mustache style templates")]
    public class TemplateSpanBuilder : IElementBuilder
    {
        public bool Matches(ElementRequest subject)
        {
            return true;
        }

        public HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("span").MustacheText(request.ElementId);
        }
    }
}