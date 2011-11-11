using FubuLocalization;
using FubuMVC.Core.UI.Configuration;
using HtmlTags;

namespace FubuMVC.Core.Localization
{
    public class LabelBuilder : ElementBuilder
    {
        protected override bool matches(AccessorDef def)
        {
            return true;
        }

        public override HtmlTag Build(ElementRequest request)
        {
            string header = LocalizationManager.GetHeader(request.Accessor.InnerProperty);
            return new HtmlTag("label").Attr("for", request.ElementId).Text(header);
        }
    }
}