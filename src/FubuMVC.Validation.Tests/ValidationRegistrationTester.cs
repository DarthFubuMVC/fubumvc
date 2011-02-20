using FubuMVC.Core;
using FubuValidation;
using FubuValidation.Registration;
using NUnit.Framework;

namespace FubuMVC.Validation.Tests
{
    [TestFixture]
    public class ValidationRegistrationTester
    {
        [Test]
        public void should_register_validation_query()
        {
            var registry = new FubuRegistry();
            registry.WithValidationDefaults();

            IValidationQuery query = null;
            registry.Services(x =>
                                  {
                                      query = (IValidationQuery)x.DefaultServiceFor<IValidationQuery>().Value;
                                  });
            registry.BuildGraph();

            query.ShouldNotBeNull();
        }

        [Test]
        public void should_register_validation_provider()
        {
            var registry = new FubuRegistry();
            registry.WithValidationDefaults();

            IValidationProvider provider = null;
            registry.Services(x =>
                                  {
                                      provider = (IValidationProvider)x.DefaultServiceFor<IValidationProvider>().Value;
                                  });

            registry.BuildGraph();
            provider.ShouldNotBeNull();
        }
    }
}