using System;
using System.ComponentModel;
using HtmlTags;

namespace FubuHtml.Elements.Builders
{
    [Description("Builds a <span>[accessor value]</span> element using IDisplayFormatter")]
    public class SpanDisplayBuilder : IElementBuilder
    {
        public bool Matches(ElementRequest subject)
        {
            return true;
        }

        public HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("span").Text(request.StringValue()).Id(request.ElementId);
        }
    }
    
}