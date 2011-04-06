using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuTestingSupport;
using FubuValidation.Fields;
using FubuValidation.Tests.Models;
using NUnit.Framework;

namespace FubuValidation.Tests.Fields
{
    [TestFixture]
    public class when_validating_maximum_length
    {

        private AddressModel theModel;
        private MaximumLengthRule theRule;


        [SetUp]
        public void BeforeEach()
        {
            theRule = new MaximumLengthRule(10);
            theModel = new AddressModel();
        }

        [Test]
        public void should_not_register_message_if_value_is_null()
        {
            theModel.Address1 = null;
            theRule.ValidateProperty(theModel, x => x.Address1).AllMessages.Any().ShouldBeFalse();
        }

        [Test]
        public void should_register_message_if_string_is_greater_than_limit()
        {
            theModel.Address1 = "Invalid property value";
            var theMessage = theRule.ValidateProperty(theModel, x => x.Address1).MessagesFor<AddressModel>(x => x.Address1)
                .Single();

            theMessage.GetMessage().ShouldEqual("Maximum length exceeded. Must be less than or equal to 10");
        }

        [Test]
        public void should_not_register_a_message_if_property_is_valid()
        {
            theModel.Address1 = "Valid";
            theRule.ValidateProperty(theModel, x => x.Address1).AllMessages.Any().ShouldBeFalse();
        }

    }
}