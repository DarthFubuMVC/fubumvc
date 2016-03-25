using System.Collections.Generic;
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

            // Done explicitly now in BehaviorGraphBuilder
            ForConcreteType<ValidationGraph>().Configure.Singleton().OnCreation("Apply all the validation registrations", (context, graph) =>
            {
                var registrations = context.GetAllInstances<IValidationRegistration>();
                registrations.Each(x => x.Register(graph));
            });

            ForSingletonOf<IFieldRulesRegistry>().Use<FieldRulesRegistry>();
            ForConcreteType<RemoteRuleGraph>().Configure.Singleton();



            For<IFieldValidationQuery>().Use<FieldValidationQuery>();

        }

    }
}