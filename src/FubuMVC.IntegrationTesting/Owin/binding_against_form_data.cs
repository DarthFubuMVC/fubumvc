using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class binding_against_form_data
    {
        [Test]
        public void can_bind_to_form_post_data()
        {
            var formInput = new FormInput
            {
                Color = "Orange",
                Direction = "South"
            };

            TestHost.Scenario(_ => {
                _.FormData(formInput);

                _.ContentShouldBe(formInput.ToString());
            });
        }

        // The following is really a test against Scenario
        [Test]
        public void can_bind_to_form_post_data_defined_explicitly()
        {
            var formInput = new FormInput
            {
                Color = "Orange",
                Direction = "South"
            };

            TestHost.Scenario(_ => {
                _.Post.Input(formInput);
                _.Request.ContentType(MimeType.HttpFormMimetype);

                _.Request.Form["Color"] = "Orange";
                _.Request.Form["Direction"] = "South";

                _.ContentShouldBe(formInput.ToString());
            });
        }
    }

    public class FormBindingEndpoint
    {
        public string post_form_values(FormInput input)
        {
            return input.ToString();
        }
    }

    public class FormInput
    {
        public string Color { get; set; }
        public string Direction { get; set; }

        public override string ToString()
        {
            return string.Format("Color: {0}, Direction: {1}", Color, Direction);
        }
    }
}