using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation.Web.UI
{
    public class MinValueModifier : InputElementModifier
    {
        protected override void modify(ElementRequest request)
        {
            ForRule<MinValueFieldRule>(request, rule => request.CurrentTag.Data("min", rule.Bounds));
        }
    }
}