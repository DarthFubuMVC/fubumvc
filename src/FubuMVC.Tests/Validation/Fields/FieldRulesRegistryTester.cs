using FubuCore.Reflection;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Tests.Validation.Models;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Validation.Fields
{
    [TestFixture]
    public class FieldRulesRegistryTester
    {
        private FieldRulesRegistry theRegistry;

        [SetUp]
        public void SetUp()
        {
            theRegistry = new FieldRulesRegistry(new IFieldValidationSource[0], new TypeDescriptorCache());
        }

        [Test]
        public void registers_the_attribute_field_validation_source_by_default()
        {
            var theAccessor = ReflectionHelper.GetAccessor<ContactModel>(x => x.FirstName);
            theRegistry.HasRule<RequiredFieldRule>(theAccessor).ShouldBeTrue();
        }
    }
}