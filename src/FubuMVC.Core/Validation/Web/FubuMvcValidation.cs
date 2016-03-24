using FubuMVC.Core.UI;
using FubuMVC.Core.Validation.Web.Remote;
using FubuMVC.Core.Validation.Web.UI;

namespace FubuMVC.Core.Validation.Web
{
    public class FubuMvcValidation : IFubuRegistryExtension
    {
        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            registry.Services.IncludeRegistry<FubuValidationServiceRegistry>();
            registry.Services.IncludeRegistry<FubuMvcValidationServices>();
            registry.Actions.FindWith<RemoteRulesSource>();
            registry.Actions.FindWith<ValidationSummarySource>();

            registry.Import<HtmlConventionRegistry>(x =>
            {
                x.Editors.Add(new FieldValidationElementModifier());
                x.Editors.Add(new RemoteValidationElementModifier());
                x.Editors.Add(new DateElementModifier());
                x.Editors.Add(new NumberElementModifier());
                x.Editors.Add(new MaximumLengthModifier());
                x.Editors.Add(new MinimumLengthModifier());
                x.Editors.Add(new RangeLengthModifier());
                x.Editors.Add(new MinValueModifier());
                x.Editors.Add(new MaxValueModifier());
                x.Editors.Add(new LocalizationLabelModifier());
				x.Editors.Add(new RegularExpressionModifier());
                
                x.Forms.Add(new FormValidationSummaryModifier());
                x.Forms.Add(new FormValidationModifier());
				x.Forms.Add(new FieldEqualityFormModifier());
				x.Forms.Add(new NotificationSerializationModifier());
            });

        }
    }
}