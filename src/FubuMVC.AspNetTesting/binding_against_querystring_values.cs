using AspNetApplication;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.AspNetTesting
{
    [TestFixture]
    public class binding_against_querystring_values
    {
        [Test]
        public void can_bind_against_querystring_parameters()
        {
            var model = new QueryStringModel{
                Color = "Green",
                Direction = "South"
            };

            TestApplication.Endpoints.GetByInput(model).ReadAsText()
                .ShouldEqual(model.ToString());
        }
    }
}