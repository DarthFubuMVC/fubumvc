using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Conneg
{
    [TestFixture]
    public class conneg_with_endpoint_that_accepts_all_formatters_and_form_posts : SharedHarnessContext
    {
        private readonly XmlJsonHtmlMessage input;
        private readonly string expectedJson;
        private readonly string expectedXml;

        public conneg_with_endpoint_that_accepts_all_formatters_and_form_posts()
        {
            input = new XmlJsonHtmlMessage
            {
                Id = Guid.NewGuid()
            };

            expectedJson = JsonUtil.ToJson(input);

            var writer = new StringWriter();
            var xmlWriter = new XmlTextWriter(writer)
            {
                Formatting = Formatting.None
            };
            new XmlSerializer(typeof (XmlJsonHtmlMessage)).Serialize(xmlWriter, input);
            expectedXml = writer.ToString();
        }

        [Test]
        public void requesting_an_unsupported_media_type_returns_406()
        {
            endpoints.PostJson(input, accept: "random/format").StatusCodeShouldBe(HttpStatusCode.NotAcceptable);
        }

        [Test]
        public void send_no_accept_header_but_treat_like_anything_is_accepted()
        {
            endpoints.PostJson(input, accept: null)
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentTypeShouldBe(MimeType.Json)
                .ReadAsJson<XmlJsonHtmlMessage>().Id.ShouldEqual(input.Id);
        }

        [Test]
        public void send_json_expecting_json()
        {
            endpoints.PostJson(input, "text/json", "text/json")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentShouldBe("text/json", expectedJson);

            endpoints.PostJson(input, "application/json", "application/json")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentShouldBe("application/json", expectedJson);

            endpoints.PostJson(input, "application/json", "application/json,text/xml")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentShouldBe("application/json", expectedJson);

            endpoints.PostJson(input, "text/json", "text/json,text/xml")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentShouldBe("text/json", expectedJson);
        }

        [Test]
        public void send_the_request_as_http_form_expect_json_back()
        {
            endpoints.PostAsForm(input, accept: "text/json")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentShouldBe("text/json", expectedJson);

            endpoints.PostAsForm(input, accept: "application/json")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentShouldBe("application/json", expectedJson);
        }

        [Test]
        public void send_the_request_as_http_form_expect_xml_back()
        {
            endpoints.PostAsForm(input, accept: "text/xml")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentShouldBe("text/xml", expectedXml);

            endpoints.PostAsForm(input, accept: "application/xml")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentShouldBe("application/xml", expectedXml);
        }

        [Test]
        public void uses_json_for_global_accept()
        {
            endpoints.PostJson(input, contentType: "text/json", accept: "*/*")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentShouldBe("application/json", expectedJson);

            endpoints.PostJson(input, "text/json", "something/weird,*/*")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentShouldBe("application/json", expectedJson);
        }

        [Test]
        public void will_accept_xml_as_an_input()
        {
            TestHost.Scenario(_ =>
            {
                _.XmlData(input);
                _.Request.Accepts("text/xml");

                _.ContentTypeShouldBe("text/xml");
                _.ContentShouldBe(expectedXml);
            });
        }
    }


    public class ConnegEndpoints
    {
        public XmlJsonHtmlMessage post_send_mixed(XmlJsonHtmlMessage message)
        {
            return message;
        }

        public XmlAndJsonOnlyMessage post_send_xmlorjson(XmlAndJsonOnlyMessage message)
        {
            return message;
        }
    }

    public interface ConnegMessage
    {
        Guid Id { get; set; }
    }

    public class XmlJsonHtmlMessage : ConnegMessage
    {
        public Guid Id { get; set; }
    }

    public class XmlAndJsonOnlyMessage : ConnegMessage
    {
        public Guid Id { get; set; }
    }
}