using Bottles;
using Bottles.Diagnostics;
using FubuFastPack.Testing.Security;
using FubuMVC.Core.Packaging;
using FubuTestingSupport;
using FubuValidation;
using FubuValidation.Fields;
using NUnit.Framework;
using StructureMap;
using StructureMap.Configuration.DSL;
using FubuFastPack.StructureMap;
using System.Linq;
using FubuCore;
using System.Collections.Generic;

namespace FubuFastPack.Testing.Validation
{
    [TestFixture]
    public class ValidationScanningTester
    {
        private Container theContainer;

        [SetUp]
        public void SetUp()
        {
            var registry = new Registry();
            registry.FubuValidationWith(IncludePackageAssemblies.Yes, GetType().Assembly);
            
            theContainer = new Container(registry);
            theContainer.GetAllInstances<IActivator>().Each(
                x => x.Activate(new IPackageInfo[0], new PackageLog()));
        }

        [Test]
        public void should_have_found_rules_from_validation_registrations_found_in_scanning()
        {
            theContainer.GetInstance<IFieldRulesRegistry>().RulesFor<Person>()
                .RulesFor<Person>(x => x.Name).Single().ShouldBeOfType<RequiredFieldRule>();
        }

        [Test]
        public void should_have_rules_from_validation_convention_found_from_scanning()
        {
            theContainer.GetInstance<IFieldRulesRegistry>().RulesFor<Case>()
                .RulesFor<Case>(x => x.Integer).Single().ShouldBeOfType<GreaterOrEqualToZeroRule>();
        }

        [Test]
        public void found_explicit_rule_from_validation_registry()
        {
            theContainer.GetInstance<IFieldRulesRegistry>().RulesFor<Case>()
                .RulesFor<Case>(x => x.Condition).Single().ShouldBeOfType<RequiredFieldRule>();
        }
    }

    public class PersonRules : ClassValidationRules<Person>
    {
        public PersonRules()
        {
            Require(x => x.Name);
        }
    }

    public class FakeValidationConventions : ValidationRegistry
    {
        public FakeValidationConventions()
        {
            ForClass<Case>(x => x.Require(o => o.Condition));

            ApplyRule<GreaterOrEqualToZeroRule>().IfPropertyType(t => t.IsIntegerBased());
        }
    }
}