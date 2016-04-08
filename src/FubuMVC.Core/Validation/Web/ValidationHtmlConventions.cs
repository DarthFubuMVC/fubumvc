using FubuMVC.Core.UI;
using FubuMVC.Core.Validation.Web.UI;

namespace FubuMVC.Core.Validation.Web
{
    public class ValidationHtmlConventions : HtmlConventionRegistry
    {
        public ValidationHtmlConventions()
        {
            Editors.Add(new FieldValidationElementModifier());
            Editors.Add(new RemoteValidationElementModifier());
            Editors.Add(new DateElementModifier());
            Editors.Add(new NumberElementModifier());
            Editors.Add(new MaximumLengthModifier());
            Editors.Add(new MinimumLengthModifier());
            Editors.Add(new RangeLengthModifier());
            Editors.Add(new MinValueModifier());
            Editors.Add(new MaxValueModifier());
            Editors.Add(new LocalizationLabelModifier());
            Editors.Add(new RegularExpressionModifier());

            Forms.Add(new FormValidationSummaryModifier());
            Forms.Add(new FormValidationModifier());
            Forms.Add(new FieldEqualityFormModifier());
            Forms.Add(new NotificationSerializationModifier());
        }
    }
}