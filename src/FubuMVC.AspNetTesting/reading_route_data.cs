using AspNetApplication;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.AspNetTesting
{
    [TestFixture]
    public class reading_route_data
    {
        [Test]
        public void bind_data_against_routing_data()
        {
            TestApplication.Endpoints.GetByInput(new RouteInput{
                Name = "Jeremy",
                Age = 38
            }).ReadAsText()
                .ShouldEqual("Name=Jeremy, Age=38");
        }
    }
}