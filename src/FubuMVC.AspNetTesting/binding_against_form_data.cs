using AspNetApplication;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.AspNetTesting
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

            TestApplication.Endpoints.PostAsForm(model).ReadAsText()
                .ShouldEqual(model.ToString());
        }
    }
}