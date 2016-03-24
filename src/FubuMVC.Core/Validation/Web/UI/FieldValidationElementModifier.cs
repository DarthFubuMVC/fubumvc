using System.Collections.Generic;
using FubuMVC.Core.UI.Elements;

namespace FubuMVC.Core.Validation.Web.UI
{
    public class FieldValidationElementModifier : InputElementModifier
    {
        protected override void modify(ElementRequest request)
        {
            var rules = request.Get<ValidationGraph>().FieldRulesFor(request.HolderType(), request.Accessor);
            var modifier = request.Get<IFieldValidationModifier>();

            rules.Each(x => modifier.ModifyFor(x, request));
        }
    }
}