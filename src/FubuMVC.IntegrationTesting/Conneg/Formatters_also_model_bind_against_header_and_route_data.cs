using FubuMVC.Core;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Conneg
{
    [TestFixture, Ignore]
    public class Formatters_also_model_bind_against_header_and_route_data
    {
        [Test]
        public void applies_model_binding_after_serialization_with_json()
        {
            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbeddedWithAutoPort())
            {
                // TODO -- need a better EndpointDriver first
            }
        }
    }

    public class RouteFilledInput
    {

        public string Name { get; set; }
    }

    public class RouteFilledEndpoint
    {
        public RouteFilledInput get_filled(RouteFilledInput input)
        {
            return input;
        }
    }
}