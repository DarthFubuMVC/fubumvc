using System.Linq;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Validation;
using FubuMVC.Tests.Validation.Models;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation
{
    
    public class NotificationMessageTester
    {
        [Fact]
        public void create_by_string()
        {
            var message = new NotificationMessage("Some message");
            message.GetMessage().ShouldBe("Some message");
        }

        [Fact]
        public void get_message_without_subsitutions()
        {
            var message = new NotificationMessage(NotificationMessageKeys.NO_SUBSTITUTION);
            message.GetMessage().ShouldBe(NotificationMessageKeys.NO_SUBSTITUTION.ToString());
        }

        [Fact]
        public void get_message_with_substitutions()
        {
            var message = new NotificationMessage(NotificationMessageKeys.SUBSTITUTE_NAME_AND_AGE)
                .AddSubstitution("Name", "Max")
                .AddSubstitution("Age", "7");

            message.GetMessage().ShouldBe("Max is 7 years old");
        }

        [Fact]
        public void prepend_property()
        {
            var message = new NotificationMessage(ValidationKeys.Required);
            message.AddAccessor(ReflectionHelper.GetAccessor<ContinuationRuleTester.ContactModel>(x => x.FirstName));
            message.AddAccessor(ReflectionHelper.GetAccessor<ContinuationRuleTester.ContactModel>(x => x.LastName));

            var property = ReflectionHelper.GetAccessor<CompositeModel>(x => x.Contact);

            var prepended = message.Prepend(property);

            prepended.ShouldNotBeTheSameAs(message);
            prepended.Accessors.Select(x => x.Name).ShouldHaveTheSameElementsAs("ContactFirstName", "ContactLastName");
            prepended.StringToken.ShouldBe(ValidationKeys.Required);
        }

        [Fact]
        public void equality_check()
        {
            var token = StringToken.FromKeyString("Test", "1...2...3");
            var v1 = TemplateValue.For("FirstName", "Joel");
            var v2 = TemplateValue.For("LastName", "Arnold");

            var message1 = new NotificationMessage(token, v1, v2);
            var message2 = new NotificationMessage(token, v1, v2);

            message1.ShouldBe(message2);
        }

        [Fact]
        public void equality_check_negative()
        {
            var token = StringToken.FromKeyString("Test", "1...2...3");
            var v1 = TemplateValue.For("FirstName", "Joel");
            var v2 = TemplateValue.For("LastName", "Arnold");

            var message1 = new NotificationMessage(token, v1, v2);
            var message2 = new NotificationMessage(token, v1);

            message1.ShouldNotBe(message2);
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