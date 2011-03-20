using System.Linq;
using FubuCore;
using FubuValidation.Registration;
using FubuValidation.Tests.Models;
using NUnit.Framework;

namespace FubuValidation.Tests
{
    [TestFixture]
    public class CompositeModelTests
    {
        //private IValidationQuery _query;
        private IValidationProvider _provider;

        [SetUp]
        public void SetUp()
        {
            Assert.Fail("NWO");
            
            //var registry = new ValidationRegistry();
            //var resolver = new TypeResolver();
            ////_query = new ValidationQuery(resolver, registry.GetConfiguredSources());
            //_provider = new ValidationProvider(resolver, _query);
        }

        [Test]
        public void should_validate_properties_on_ancillary_models()
        {
            var model = new CompositeModel {Contact = new ContactModel()};
            var notification = _provider.Validate(model);
            var messages = notification.AllMessages;

            var firstName = AccessorFactory.Create<ContactModel>(m => m.FirstName);
            messages.ShouldContain(m => m.Accessors.Any(a => a.Equals(firstName)));
        }
    }
}