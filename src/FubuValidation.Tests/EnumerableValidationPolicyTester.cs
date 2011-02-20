using FubuValidation.Registration.Policies;
using FubuValidation.Tests.Models;
using NUnit.Framework;

namespace FubuValidation.Tests
{
    [TestFixture]
    public class EnumerableValidationPolicyTester
    {
        private EnumerableValidationPolicy _policy;

        [SetUp]
        public void SetUp()
        {
            _policy = new EnumerableValidationPolicy();
        }

        [Test]
        public void should_match_enumerable_properties()
        {
            var prop = AccessorFactory.Create<ContactModel>(m => m.Addresses);
            _policy
                .Matches(prop)
                .ShouldBeTrue();
        }

        [Test]
        public void should_not_match_strings()
        {
            var prop = AccessorFactory.Create<ContactModel>(m => m.FirstName);
            _policy
                .Matches(prop)
                .ShouldBeFalse();
        }

        [Test]
        public void should_not_match_non_enumerable_properties()
        {
            var prop = AccessorFactory.Create<CompositeModel>(m => m.Contact);
            _policy
                .Matches(prop)
                .ShouldBeFalse();
        }
    }
}