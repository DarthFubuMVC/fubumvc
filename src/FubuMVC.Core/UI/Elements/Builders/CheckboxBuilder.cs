using System.ComponentModel;
using HtmlTags;
using FubuCore;

namespace FubuMVC.Core.UI.Elements.Builders
{
    [Description("Builds an html checkbox for a boolean value")]
    public class CheckboxBuilder : ElementTagBuilder
    {
        public override bool Matches(ElementRequest subject)
        {
            return subject.Accessor.PropertyType == typeof(bool);
        }

        public override HtmlTag Build(ElementRequest request)
        {
            return new CheckboxTag(request.RawValue.As<bool>());
        }
    }
}