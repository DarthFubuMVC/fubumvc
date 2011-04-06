using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;
using FubuTestingSupport;
using FubuValidation.Fields;
using FubuValidation.Tests.Models;
using NUnit.Framework;

namespace FubuValidation.Tests
{
    [TestFixture]
    public class ContinuationRuleTester
    {
        private IValidator _provider;

        [SetUp]
        public void SetUp()
        {
            var fieldRegistry = new FieldRulesRegistry(new IFieldValidationSource[0], new TypeDescriptorCache());
            _provider = new Validator(new TypeResolver(), new IValidationSource[] { new FieldRuleSource(fieldRegistry) });
        }

        [Test]
        public void should_validate_properties_on_ancillary_models_marked_as_continue()
        {
            var model = new CompositeModelWithAttribute {Contact = new ContactModel()};
            var notification = _provider.Validate(model);

            notification.MessagesFor<CompositeModelWithAttribute>(x => x.Contact.FirstName).ShouldHaveCount(1);
            notification.MessagesFor<CompositeModelWithAttribute>(x => x.Contact.LastName).ShouldHaveCount(1);
        }

        public class CompositeModelWithAttribute
        {
            public int Id { get; set; }

            [ContinueValidation]
            public ContactModel Contact { get; set; }
        }

        public class ContactModel
        {
            public ContactModel()
            {
                Addresses = new List<AddressModel>();
            }

            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }

            [CollectionLength(1)]
            public IEnumerable<AddressModel> Addresses { get; set; }
        }
    }
}