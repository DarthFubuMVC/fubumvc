using FubuCore.Reflection;
using FubuTestingSupport;
using FubuValidation.Fields;
using FubuValidation.Tests.Models;
using NUnit.Framework;

namespace FubuValidation.Tests.Fields
{
    [TestFixture]
    public class FieldValidationQueryTester
    {
        private FieldValidationQuery _query;

        [SetUp]
        public void setup()
        {
            var registry = new FieldRulesRegistry(new[] {new AttributeFieldValidationSource()},
                                                  new TypeDescriptorCache());
            _query = new FieldValidationQuery(registry);
        }

        [Test]
        public void should_find_basic_rules()
        {
            _query
                .RulesFor<AddressModel>(m => m.Address1)
                .ShouldHaveCount(1);
        }

        [Test]
        public void should_have_basic_rules()
        {
            _query
                .HasRule<RequiredFieldRule>(typeof(AddressModel), ReflectionHelper.GetAccessor<AddressModel>(a => a.Address1))
                .ShouldBeTrue();
        }

        [Test]
        public void should_continue_and_find_rules_for_ancillary_objects()
        {
            _query
                .RulesFor<CompositeModel>(m => m.Contact.FirstName)
                .ShouldHaveCount(1);
        }

        [Test]
        public void should_continue_and_find_rules_for_nested__continued_ancillary_objects()
        {
            _query
                .RulesFor<UserModel>(m => m.Site.Contact.FirstName)
                .ShouldHaveCount(1);
        }

        [Test]
        public void should_not_get_rules_for_ancillary_objects_without_a_continue_validation()
        {
            _query
                .RulesFor<CompositeModel>(m => m.RestrictedContact.FirstName)
                .ShouldHaveCount(0);
        }

        [Test]
        public void should_not_get_rules_for_nested_ancillary_objects_without_a_continue_validation()
        {
            _query
                .RulesFor<UserModel>(m => m.Site.AlternateContact.FirstName)
                .ShouldHaveCount(0);
        }

        [Test]
        public void should_call_continuation_for_rules()
        {
            var found = false;
            var accessor = ReflectionHelper.GetAccessor<CompositeModel>(m => m.Contact.FirstName);
            _query
                .ForRule<RequiredFieldRule>(typeof(CompositeModel), accessor, rule => { found = true; });
            found.ShouldBeTrue();
        }

        [Test]
        public void should_not_get_rules_for_ancillary_objects_in_collection()
        {
            _query
                .RulesFor<CompositeModel>(m => m.Contacts[0])
                .ShouldHaveCount(0);
        }
    }
}