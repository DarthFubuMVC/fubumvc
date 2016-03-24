using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation.Web.UI
{
    public class RangeLengthModifier : InputElementModifier
    {
        protected override void modify(ElementRequest request)
        {
            ForRule<RangeLengthFieldRule>(request, rule => request.CurrentTag.Data("rangelength", rule.ToValues()));
        }
    }
}