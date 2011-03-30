using System.Linq;
using FubuTestingSupport;
using FubuValidation.Fields;
using FubuValidation.Tests.Models;
using NUnit.Framework;

namespace FubuValidation.Tests.Fields
{
    [TestFixture]
    public class when_validating_required_fields
    {
        #region Setup/Teardown

        [SetUp]
        public void BeforeEach()
        {
            theRule = new RequiredFieldRule();
            theTarget = new AddressModel();
        }

        #endregion

        private AddressModel theTarget;
        private RequiredFieldRule theRule;

        [Test]
        public void should_not_register_a_message_if_property_is_valid()
        {
            theTarget.Address1 = "1234 Test Lane";
            theRule.ValidateProperty(theTarget, x => x.Address1)
                .MessagesFor<AddressModel>(x => x.Address1).Any().ShouldBeFalse();
        }

        [Test]
        public void should_register_message_if_string_value_is_empty()
        {
            theTarget.Address1 = "";
            theRule.ValidateProperty(theTarget, x => x.Address1)
                .MessagesFor<AddressModel>(x => x.Address1).Single().StringToken.ShouldEqual(
                    ValidationKeys.REQUIRED);
        }

        [Test]
        public void should_register_message_if_value_is_null()
        {
            theTarget.Address1 = null;
            theRule.ValidateProperty(theTarget, x => x.Address1)
                .MessagesFor<AddressModel>(x => x.Address1).Single().StringToken.ShouldEqual(
                    ValidationKeys.REQUIRED);
        }
    }
}