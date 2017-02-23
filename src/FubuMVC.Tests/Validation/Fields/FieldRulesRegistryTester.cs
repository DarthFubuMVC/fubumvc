using FubuCore.Reflection;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Tests.Validation.Models;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Fields
{
    
    public class FieldRulesRegistryTester
    {
        private FieldRulesRegistry theRegistry = new FieldRulesRegistry(new IFieldValidationSource[0], new TypeDescriptorCache());

        [Fact]
        public void registers_the_attribute_field_validation_source_by_default()
        {
            var theAccessor = ReflectionHelper.GetAccessor<ContactModel>(x => x.FirstName);
            theRegistry.HasRule<RequiredFieldRule>(theAccessor).ShouldBeTrue();
        }
    }
}