using System.Linq;
using FubuCore;
using FubuValidation.Registration;
using FubuValidation.Strategies;
using FubuValidation.Tests.Models;
using NUnit.Framework;

namespace FubuValidation.Tests
{
    [TestFixture]
    public class ExplicitValidationTests
    {
        private IValidationQuery _query;
        private IValidationProvider _provider;

        [SetUp]
        public void SetUp()
        {
            var validationRegistry = new ValidationRegistry(registry => registry
                                                                            .Rules
                                                                            .For<ModelWithNoAttributes>()
                                                                            .ApplyStrategy<RequiredFieldStrategy>(
                                                                                m => m.SomeRequiredString)
                                                                            .ApplyStrategy<GreaterThanZeroFieldStrategy>(
                                                                                m => m.Id)
                                                                            .ApplyStrategy<GreaterThanZeroFieldStrategy>(
                                                                                m => m.AncillaryId));
            _query = validationRegistry.BuildQuery();
            _provider = new ValidationProvider(new TypeResolver(), _query);
        }

        [Test]
        public void should_apply_strategies_by_expression()
        {
            var model = new ModelWithNoAttributes();
            var notification = _provider.Validate(model);
            var messages = notification.AllMessages;

            var id = AccessorFactory.Create<ModelWithNoAttributes>(m => m.Id);
            var ancillaryId = AccessorFactory.Create<ModelWithNoAttributes>(m => m.AncillaryId);
            var stringProp = AccessorFactory.Create<ModelWithNoAttributes>(m => m.SomeRequiredString);

            messages
                .ShouldContain(m => m.Accessors.Any(a => a.Equals(id)));
            messages
                .ShouldContain(m => m.Accessors.Any(a => a.Equals(ancillaryId)));
            messages
                .ShouldContain(m => m.Accessors.Any(a => a.Equals(stringProp)));
        }
    }
}