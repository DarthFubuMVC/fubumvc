using System.Linq;
using FubuCore;
using FubuValidation.Registration;
using FubuValidation.Strategies;
using FubuValidation.Tests.Models;
using NUnit.Framework;

namespace FubuValidation.Tests
{
    [TestFixture]
    public class ConventionalValidationTests
    {
        private IValidationQuery _query;
        private IValidationProvider _provider;

        [SetUp]
        public void SetUp()
        {
            var validationRegistry = new ValidationRegistry(registry =>
                                                                {
                                                                    registry
                                                                        .Rules
                                                                        .IfPropertyIs<int>()
                                                                        .ApplyStrategy<GreaterThanZeroFieldStrategy>();

                                                                    registry
                                                                        .Rules
                                                                        .If(a => a.Name.ToLower().Contains("required"))
                                                                        .ApplyStrategy<RequiredFieldStrategy>();
                                                                });
            var resolver = new TypeResolver();
            _query = new ValidationQuery(resolver, validationRegistry.GetConfiguredSources());
            _provider = new ValidationProvider(resolver, _query);
        }

        [Test]
        public void should_apply_strategies_by_property_type()
        {
            var model = new ModelWithNoAttributes();
            var notification = _provider.Validate(model);
            var messages = notification.AllMessages;

            var id = AccessorFactory.Create<ModelWithNoAttributes>(m => m.Id);
            var ancillaryId = AccessorFactory.Create<ModelWithNoAttributes>(m => m.AncillaryId);

            messages
                .ShouldContain(m => m.Accessors.Any(a => a.Equals(id)));

            messages
                .ShouldContain(m => m.Accessors.Any(a => a.Equals(ancillaryId)));
        }

        [Test]
        public void should_apply_strategies_by_predicate()
        {
            var model = new ModelWithNoAttributes();
            var notification = _provider.Validate(model);
            var messages = notification.AllMessages;

            var stringProp = AccessorFactory.Create<ModelWithNoAttributes>(m => m.SomeRequiredString);

            messages
                .ShouldContain(m => m.Accessors.Any(a => a.Equals(stringProp)));
        }
    }
}