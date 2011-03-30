using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuFastPack.Extensibility;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuFastPack.Testing.Extensibility
{
    [TestFixture]
    public class ExtensionPropertyBinderTester
    {
        [Test]
        public void matches_extension_properties_on_domain_entities()
        {
            var propertyToCheck = ReflectionHelper.GetProperty<FakeEntity>(x => x.ExtendedProperties);
            new ExtensionPropertyBinder().Matches(propertyToCheck).ShouldBeTrue();
        }

        [Test]
        public void does_not_match_other_properties_of_domain_entity()
        {
            var propertyToCheck = ReflectionHelper.GetProperty<FakeEntity>(x => x.Path);
            new ExtensionPropertyBinder().Matches(propertyToCheck).ShouldBeFalse();
        }

        [Test]
        public void does_not_match_extension_property_on_non_domain_entities()
        {
            var propertyToCheck = ReflectionHelper.GetProperty<NonDomainEntity>(x => x.ExtendedProperties);
            new ExtensionPropertyBinder().Matches(propertyToCheck).ShouldBeFalse();
        }

        class FakeEntity : DomainEntity
        {
            public string Name { get; set; }
        }

        class FakeEntityExtensions : Extends<FakeEntity>
        {
            public int Age { get; set; }
        }

        class NonDomainEntity
        {
            public object ExtendedProperties { get; set; }
        }
    }
}