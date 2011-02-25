using System.Linq;
using FubuMVC.Core;
using FubuValidation.Registration;
using FubuValidation.Registration.Sources;
using NUnit.Framework;

namespace FubuMVC.Validation.Tests
{
    [TestFixture]
    public class ValidationRegistrationTester
    {
        [Test]
        public void should_register_validation_policy_source()
        {
            var registry = new FubuRegistry();
            registry.WithValidationDefaults();

            var isRegistered = false;
            registry.Services(x =>
                                  {
                                      var sources = x.ServicesFor<IValidationSource>();
                                      isRegistered = sources.Any(def => def.Value.GetType() == typeof(ValidationPolicySource));
                                  });
            registry.BuildGraph();

            isRegistered.ShouldBeTrue();
        }
    }
}