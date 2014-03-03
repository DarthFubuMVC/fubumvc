using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class reading_route_data
    {
        [Test]
        public void bind_data_against_routing_data()
        {
            HarnessApplication.Run(x => {
                x.GetByInput(new RouteInput{
                Name = "Jeremy",
                Age = 38
            }).ReadAsText()
                .ShouldEqual("Name=Jeremy, Age=38");
            });

        }
    }

    public class ReadingRouteEndpoint
    {
        public string get_routing_data_Name_Age(RouteInput input)
        {
            return "Name={0}, Age={1}".ToFormat(input.Name, input.Age);
        }
    }

    public class RouteInput
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}