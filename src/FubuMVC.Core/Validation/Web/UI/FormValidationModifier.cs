using FubuCore;
using FubuMVC.Core.UI.Forms;
using HtmlTags.Conventions;

namespace FubuMVC.Core.Validation.Web.UI
{
    public class FormValidationModifier : ITagModifier<FormRequest>
    {
        public bool Matches(FormRequest token)
        {
            return true;
        }

        public void Modify(FormRequest request)
        {
            var validation = request.Chain.ValidationNode();
            if (validation.IsEmpty())
            {
                return;
            }

            var node = validation.As<IValidationNode>();
            node.Modify(request);

            var options = ValidationOptions.For(request);
            request.CurrentTag.Data(ValidationOptions.Data, options);

            request.CurrentTag.AddClass("validated-form");
        }
    }
}