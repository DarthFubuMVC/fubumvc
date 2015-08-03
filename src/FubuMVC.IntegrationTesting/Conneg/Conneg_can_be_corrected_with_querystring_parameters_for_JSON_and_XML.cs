using System;
using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Runtime;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Conneg
{
    [TestFixture]
    public class Conneg_can_be_corrected_with_querystring_parameters_for_JSON_and_XML : SharedHarnessContext
    {
        [Test]
        public void force_to_json_with_querystring()
        {
            endpoints.Get("conneg/override/Foo?Format=Json", "text/html")
                .ContentTypeShouldBe(MimeType.Json)
                .ReadAsJson<OverriddenResponse>()
                .Name.ShouldBe("Foo");
        }

        [Test]
        public void force_to_xml_with_querystring()
        {
            endpoints.Get("conneg/override/Foo?Format=Xml", "text/html")
                .ContentTypeShouldBe(MimeType.Xml);
        }

        [Test]
        public void force_to_json_with_querystring_2()
        {
            endpoints.Get("conneg/override/Foo?format=JSON", "text/html")
                .ContentTypeShouldBe(MimeType.Json)
                .ReadAsJson<OverriddenResponse>()
                .Name.ShouldBe("Foo");
        }

        [Test]
        public void force_to_xml_with_querystring_2()
        {
            endpoints.Get("conneg/override/Foo?format=xml", "text/html")
                .ContentTypeShouldBe(MimeType.Xml);
        }
    }

    [TestFixture]
    public class ExampleTests
    {
        [Test]
        public void with_Katana_and_EndpointDriver()
        {
            using (var server = FubuRuntime.Basic(_ => _.HostWith<Katana>()))
            {
                server.Endpoints.Get("conneg/override/Foo?format=json", "text/html")
                    .ContentTypeShouldBe(MimeType.Json)
                    .ReadAsJson<OverriddenResponse>()
                    .Name.ShouldBe("Foo");
            }
        }

        [Test]
        public void with_in_memory_host()
        {
            // The 'Scenario' testing API was not completed,
            // so I never got around to creating more convenience
            // methods for common things like deserializing JSON
            // into .Net objects from the response body
            using (var host = FubuRuntime.Basic())
            {
                host.Scenario(_ =>
                {
                    _.Get.Url("conneg/override/Foo?format=json");
                    _.Request.Accepts("text/html");

                    _.ContentTypeShouldBe(MimeType.Json);
                    _.Response.Body.ReadAsText()
                        .ShouldBe("{\"Name\":\"Foo\"}");
                });
            }
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