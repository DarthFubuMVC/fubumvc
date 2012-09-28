using System;
using System.ComponentModel;
using HtmlTags;
using FubuCore;

namespace FubuHtml.Elements.Builders
{
    [Description("Builds an html checkbox for a boolean value")]
    public class CheckboxBuilder : IElementBuilder
    {
        public bool Matches(ElementRequest subject)
        {
            return subject.Accessor.PropertyType == typeof(bool);
        }

        public HtmlTag Build(ElementRequest request)
        {
            return new CheckboxTag(request.RawValue.As<bool>());
        }
    }
}