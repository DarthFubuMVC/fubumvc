using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuValidation.Registration;
using FubuValidation.Tests.Models;
using NUnit.Framework;

namespace FubuValidation.Tests
{
    [TestFixture]
    public class ModelWithEnumerablePropertyTests
    {
        private IValidationQuery _query;
        private IValidationProvider _provider;

        [SetUp]
        public void SetUp()
        {
            var registry = new ValidationRegistry();
            _query = registry.BuildQuery();
            _provider = new ValidationProvider(new TypeResolver(), _query);
        }

        [Test]
        public void should_validate_each_object_within_an_enumerable_property()
        {
            var model = new ContactModel
                            {
                                FirstName = "Test",
                                LastName = "Contact",
                                Addresses = new List<AddressModel>
                                                {
                                                    new AddressModel // Missing country
                                                        {
                                                            Address1 = "1234 Test Lane",
                                                            City = "Austin",
                                                            StateOrProvince = "TX",
                                                            PostalCode = "78701"
                                                        },
                                                    new AddressModel // Missing postal code
                                                        {
                                                            Address1 = "1234 Test Lane",
                                                            City = "Austin",
                                                            StateOrProvince = "TX",
                                                            Country = "US"
                                                        }
                                                }
                            };
            
            var notification = _provider.Validate(model);
            var messages = notification.AllMessages;

            var country = AccessorFactory.Create<AddressModel>(m => m.Country);
            var postalCode = AccessorFactory.Create<AddressModel>(m => m.PostalCode);

            messages.ShouldContain(m => m.Accessors.Any(a => a.Equals(country)));
            messages.ShouldContain(m => m.Accessors.Any(a => a.Equals(postalCode)));
        }

        [Test]
        public void should_validate_collection_length_for_enumerable_property()
        {
            var model = new ContactModel
                            {
                                FirstName = "Test",
                                LastName = "Contact"
                            };

            var notification = _provider.Validate(model);
            var messages = notification.AllMessages;
            var addresses = AccessorFactory.Create<ContactModel>(m => m.Addresses);

            messages
                .ShouldContain(m => m.Accessors.Any(a => a.Equals(addresses)));
        }
    }
}