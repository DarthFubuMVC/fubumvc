using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web.Remote;

namespace FubuMVC.Core.Validation.Web
{
    public class FubuValidationServiceRegistry : ServiceRegistry
    {
        public FubuValidationServiceRegistry()
        {
            SetServiceIfNone<ITypeResolver, TypeResolver>();
            SetServiceIfNone<IValidator, Validator>();

            AddService<IFieldValidationSource, AccessorRulesFieldSource>();

            ForConcreteType<ValidationGraph>().Configure.Singleton();
            ForSingletonOf<IFieldRulesRegistry>().Use<FieldRulesRegistry>();
            ForConcreteType<RemoteRuleGraph>().Configure.Singleton();
        }

    }
}