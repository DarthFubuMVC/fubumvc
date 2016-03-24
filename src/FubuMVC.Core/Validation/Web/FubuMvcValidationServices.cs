using FubuMVC.Core.Registration;
using FubuMVC.Core.Validation.Web.Remote;
using FubuMVC.Core.Validation.Web.UI;

namespace FubuMVC.Core.Validation.Web
{
    public class FubuMvcValidationServices : ServiceRegistry
    {
        public FubuMvcValidationServices()
        {
            SetServiceIfNone<IAjaxContinuationResolver, AjaxContinuationResolver>();
            SetServiceIfNone<IModelBindingErrors, ModelBindingErrors>();
            SetServiceIfNone<IAjaxValidationFailureHandler, AjaxValidationFailureHandler>();
            SetServiceIfNone<IValidationTargetResolver, ValidationTargetResolver>();
            SetServiceIfNone<IRuleRunner, RuleRunner>();
            SetServiceIfNone(typeof(IValidationFilter<>), typeof(ValidationFilter<>));
            SetServiceIfNone<IFieldValidationModifier, FieldValidationModifier>();

            // Order is kind of important here

            AddService<IActivator, RemoteRuleGraphActivator>();
            AddService<IValidationAnnotationStrategy, CssValidationAnnotationStrategy>();
			AddService<IValidationAnnotationStrategy, LocalizationAnnotationStrategy>();

            For<IRemoteRuleQuery>().Use<RemoteRuleQuery>();
        }
    }
}