using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation.Web.UI
{
    public class MaximumLengthModifier : InputElementModifier
    {
        protected override void modify(ElementRequest request)
        {
            ForRule<MaximumLengthRule>(request, rule => request.CurrentTag.Attr("maxlength", rule.Length));
        }
    }
}