using FubuCore.Reflection;
using FubuLocalization;
using FubuTestingSupport;
using FubuValidation.Tests.Models;
using NUnit.Framework;
using System.Linq;

namespace FubuValidation.Tests
{
    [TestFixture]
    public class NotificationMessageTester
    {
        [Test]
        public void get_message_without_subsitutions()
        {
            var message = new NotificationMessage(NotificationMessageKeys.NO_SUBSTITUTION);
            message.GetMessage().ShouldEqual(NotificationMessageKeys.NO_SUBSTITUTION.ToString());
        }

        [Test]
        public void get_message_with_substitutions()
        {
            var message = new NotificationMessage(NotificationMessageKeys.SUBSTITUTE_NAME_AND_AGE)
                .AddSubstitution("Name", "Max")
                .AddSubstitution("Age", "7");

            message.GetMessage().ShouldEqual("Max is 7 years old");
        }

        [Test]
        public void prepend_property()
        {
            var message = new NotificationMessage(ValidationKeys.REQUIRED);
            message.AddAccessor(ReflectionHelper.GetAccessor<ContactModel>(x => x.FirstName));
            message.AddAccessor(ReflectionHelper.GetAccessor<ContactModel>(x => x.LastName));

            var property = ReflectionHelper.GetAccessor<CompositeModel>(x => x.Contact);

            var prepended = message.Prepend(property);

            prepended.ShouldNotBeTheSameAs(message);
            prepended.Accessors.Select(x => x.Name).ShouldHaveTheSameElementsAs("ContactFirstName", "ContactLastName");
            prepended.StringToken.ShouldEqual(ValidationKeys.REQUIRED);
        }
    }



    public class NotificationMessageKeys : StringToken
    {
        protected NotificationMessageKeys(string key, string defaultValue) : base(key, defaultValue)
        {
        }

        public static NotificationMessageKeys NO_SUBSTITUTION = new NotificationMessageKeys("NO_SUBSTITUTION", "Just as it is");
        public static NotificationMessageKeys SUBSTITUTE_NAME_AND_AGE = new NotificationMessageKeys("SUBSTITUTE_NAME_AND_AGE", "{Name} is {Age} years old");
    }
}