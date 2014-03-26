using System.ComponentModel;
using HtmlTags;

namespace FubuMVC.Core.UI.Elements.Builders
{
    [Description("Builds a textbox element for mustache style templates")]
    public class TemplateTextboxBuilder : IElementBuilder
    {
        public bool Matches(ElementRequest subject)
        {
            return true;
        }

        public HtmlTag Build(ElementRequest request)
        {
            return new TextboxTag(request.ElementId, request.ElementId.Mustached());
        }
    }
}