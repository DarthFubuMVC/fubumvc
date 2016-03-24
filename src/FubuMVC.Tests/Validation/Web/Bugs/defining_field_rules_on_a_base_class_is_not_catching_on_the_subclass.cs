using FubuCore.Descriptions;
using FubuMVC.Core;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.Bugs
{
    [TestFixture]
    public class defining_field_rules_on_a_base_class_is_not_catching_on_the_subclass
    {
        [Test, Explicit]
        public void should_be_able_to_resolve_the_field_rules()
        {
            using (var runtime = FubuRuntime.Basic(_ => _.Import<FubuMvcValidation>()))
            {
                var graph = runtime.Get<ValidationGraph>();

                var plan = graph.PlanFor(typeof (MySubclass));

                plan.FieldRules.RulesFor<MySubclass>(x => x.FirstName).Count().ShouldBe(2);

                plan.WriteDescriptionToConsole();

                var query = runtime.Get<FieldValidationQuery>();
                query.RulesFor<MySubclass>(x => x.FirstName).Count().ShouldBe(2);
            }
        }
    }

    public class MyBase
    {
        public string FirstName { get; set; }
    }

    public class MySubclass : MyBase
    {
        
    }

    public class MySubclassRules : MyBaseRules<MySubclass>
    {
        public MySubclassRules()
        {
            Property(x => x.FirstName).MaximumLength(50);
        }
    }

    public class MyBaseRules<T> : ClassValidationRules<T> where T : MyBase
    {
        public MyBaseRules()
        {
            Require(x => x.FirstName);
        }
    }
}