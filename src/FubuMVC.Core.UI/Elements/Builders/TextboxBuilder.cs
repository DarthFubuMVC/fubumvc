using System;
using System.ComponentModel;
using HtmlTags;

namespace FubuHtml.Elements.Builders
{
    [Description("Builds an html textbox with the value equal to the string representation of the accessor value")]
    public class TextboxBuilder : IElementBuilder
    {
        public bool Matches(ElementRequest subject)
        {
            return true;
        }

        public HtmlTag Build(ElementRequest request)
        {
            return new TextboxTag().Attr("value", request.RawValue as string ?? string.Empty);
        }
    }
}