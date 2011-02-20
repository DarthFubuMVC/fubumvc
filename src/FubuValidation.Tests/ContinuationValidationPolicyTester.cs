using FubuValidation.Registration.Policies;
using FubuValidation.Tests.Models;
using NUnit.Framework;

namespace FubuValidation.Tests
{
    [TestFixture]
    public class ContinuationValidationPolicyTester
    {
        private ContinuationValidationPolicy _policy;

        [SetUp]
        public void SetUp()
        {
            _policy = new ContinuationValidationPolicy();
        }

        [Test]
        public void should_not_match_primitive_properties()
        {
            var prop = AccessorFactory.Create<CompositeModel>(m => m.Id);

            _policy
                .Matches(prop)
                .ShouldBeFalse();
        }

        [Test]
        public void should_not_match_enumerable_properties()
        {
            var prop = AccessorFactory.Create<ContactModel>(m => m.Addresses);

            _policy
                .Matches(prop)
                .ShouldBeFalse();
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
        public void should_match_other_types()
        {
            var prop = AccessorFactory.Create<CompositeModel>(m => m.Contact);

            _policy
                .Matches(prop)
                .ShouldBeTrue();
        }
    }
}