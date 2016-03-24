using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation.Web.UI
{
    public class MinimumLengthModifier : InputElementModifier
    {
        protected override void modify(ElementRequest request)
        {
            ForRule<MinimumLengthRule>(request, rule => request.CurrentTag.Data("minlength", rule.Length));
        }
    }
}