using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Runtime;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Conneg
{
    [TestFixture]
    public class Conneg_can_be_corrected_with_querystring_parameters_for_JSON_and_XML : SharedHarnessContext
    {
        [Test]
        public void force_to_json_with_querystring()
        {
            endpoints.Get("conneg/override/Foo?Format=Json", acceptType: "text/html")
                .ContentTypeShouldBe(MimeType.Json)
                .ReadAsJson<OverriddenResponse>()
                .Name.ShouldEqual("Foo");
        }

        [Test]
        public void force_to_xml_with_querystring()
        {
            endpoints.Get("conneg/override/Foo?Format=Xml", acceptType: "text/html")
                .ContentTypeShouldBe(MimeType.Xml);
        }

        [Test]
        public void force_to_json_with_querystring_2()
        {
            endpoints.Get("conneg/override/Foo?format=JSON", acceptType: "text/html")
                .ContentTypeShouldBe(MimeType.Json)
                .ReadAsJson<OverriddenResponse>()
                .Name.ShouldEqual("Foo");
        }

        [Test]
        public void force_to_xml_with_querystring_2()
        {
            endpoints.Get("conneg/override/Foo?format=xml", acceptType: "text/html")
                .ContentTypeShouldBe(MimeType.Xml);
        }
    }

    [TestFixture]
    public class ExampleTests
    {
        [Test]
        public void with_Katana_and_EndpointDriver()
        {
            using (var server = EmbeddedFubuMvcServer
                .For<SampleApplication>(port: 5700))
            {
                server.Endpoints.Get("conneg/override/Foo?format=json", acceptType: "text/html")
                    .ContentTypeShouldBe(MimeType.Json)
                    .ReadAsJson<OverriddenResponse>()
                    .Name.ShouldEqual("Foo");
            }
        }

        [Test]
        public void with_in_memory_host()
        {
            // The 'Scenario' testing API was not completed,
            // so I never got around to creating more convenience
            // methods for common things like deserializing JSON
            // into .Net objects from the response body
            using (var host = InMemoryHost.For<SampleApplication>())
            {
                host.Scenario(_ => {
                    _.Get.Url("conneg/override/Foo?format=json");
                    _.Request.Accepts("text/html");

                    _.ContentTypeShouldBe(MimeType.Json);
                    _.Response.Body.ReadAsText()
                        .ShouldEqual("{\"Name\":\"Foo\"}");
                });
            }
        }
    }

    public class SampleApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication.DefaultPolicies().StructureMap();
        }
    }

    public class OverriddenConnegEndpoint
    {
        public OverriddenResponse get_conneg_override_Name(OverriddenResponse response)
        {
            return response;
        }
    }

    public class OverriddenResponse
    {
        public string Name { get; set; }
    }
}