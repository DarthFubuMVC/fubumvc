using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation.Web.UI
{
    public class MaxValueModifier : InputElementModifier
    {
        protected override void modify(ElementRequest request)
        {
            ForRule<MaxValueFieldRule>(request, rule => request.CurrentTag.Data("max", rule.Bounds));
        }
    }
}