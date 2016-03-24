using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation.Web.UI
{
    public interface IValidationAnnotationStrategy
    {
        bool Matches(IFieldValidationRule rule);
        void Modify(ElementRequest request, IFieldValidationRule rule);
    }
}