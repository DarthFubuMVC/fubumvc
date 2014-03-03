using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
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

            Harness.Endpoints.GetByInput(model).ReadAsText()
                .ShouldEqual(model.ToString());
        }
    }

    public class QueryStringEndpoint
    {
        public string get_querystring_data(QueryStringModel model)
        {
            return model.ToString();
        }
    }

    public class QueryStringModel
    {
        [QueryString]
        public string Color { get; set; }

        [QueryString]
        public string Direction { get; set; }

        public override string ToString()
        {
            return string.Format("Color: {0}, Direction: {1}", Color, Direction);
        }
    }
}