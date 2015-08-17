using System;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.IntegrationTesting.Conneg
{
    [Obsolete("Remove this after the ST specs are done")]
    [TestFixture]
    public class Conneg_can_be_corrected_with_querystring_parameters_for_JSON_and_XML
    {
        [Test]
        public void force_to_json_with_querystring()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Url("conneg/override/Foo?Format=Json").Accepts("text");
                _.Response.Body.ReadAsJson<OverriddenResponse>()
                    .Name.ShouldBe("Foo");
            });
        }

        [Test]
        public void force_to_xml_with_querystring()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Url("conneg/override/Foo?Format=Xml").Accepts("text/html");
                _.ContentTypeShouldBe(MimeType.Xml);
            });
        }

        [Test]
        public void force_to_json_with_querystring_2()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Url("conneg/override/Foo?format=JSON").Accepts("text/html");

                _.ContentTypeShouldBe(MimeType.Json);
                _.Response.Body.ReadAsJson<OverriddenResponse>()
                    .Name.ShouldBe("Foo");
            });
        }

        [Test]
        public void force_to_xml_with_querystring_2()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Url("conneg/override/Foo?format=xml").Accepts("text/html");
                _.ContentTypeShouldBe(MimeType.Xml);
            });
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