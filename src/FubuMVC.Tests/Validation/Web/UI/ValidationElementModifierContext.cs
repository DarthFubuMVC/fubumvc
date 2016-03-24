using FubuMVC.Core.UI.Elements;
using HtmlTags;

namespace FubuMVC.Tests.Validation.Web.UI
{
    public abstract class ValidationElementModifierContext<T> where T : IElementModifier, new()
    {
        protected HtmlTag tagFor(ElementRequest request)
        {
            return ValidationElementModifierScenario<T>.For(x => { x.Request = request; }).Tag;
        }
    }
}