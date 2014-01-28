using FubuMVC.Core.Runtime;
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