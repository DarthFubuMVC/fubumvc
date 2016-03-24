using FubuCore;
using FubuMVC.Core.UI.Elements;

namespace FubuMVC.Core.Validation.Web.UI
{
    public class NumberElementModifier : InputElementModifier
    {
        public override bool Matches(ElementRequest token)
        {
            return token.Accessor.PropertyType.IsNumeric();
        }

        protected override void modify(ElementRequest request)
        {
            request.CurrentTag.AddClass("number");
        }
    }
}