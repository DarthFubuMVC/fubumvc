using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class binding_against_form_data
    {
        [Test]
        public void can_bind_to_form_post_data()
        {
            var model = new FormInput{
                Color = "Orange",
                Direction = "South"
            };

            Harness.Endpoints.PostAsForm(model).ReadAsText()
                .ShouldEqual(model.ToString());
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