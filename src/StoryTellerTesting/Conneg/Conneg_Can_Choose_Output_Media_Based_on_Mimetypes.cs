using System;
using System.Net;
using System.Xml.Serialization;
using FubuMVC.Core;
using HtmlTags;
using NUnit.Framework;
using IntegrationTesting.Conneg;

namespace IntegrationTesting.Conneg
{
    [TestFixture]
    public class asymmetric_json_endpoints_with_conneg : FubuRegistryHarness
    {
        private readonly AsymmetricJson input;
        private string expectedJson;

        public asymmetric_json_endpoints_with_conneg()
        {
            input = new AsymmetricJson{
                Id = Guid.NewGuid()
            };

            expectedJson = JsonUtil.ToJson(input);
        }

        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<ConnegController>();
            registry.Media // TODO -- I really don't like that you have to do this.
                .ApplyContentNegotiationToActions(call => true);
        }

        [Test]
        public void send_json_expecting_json()
        {
            endpoints.PostJson(input, contentType: "text/json", accept: "text/json")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentShouldBe("text/json", expectedJson);

            endpoints.PostJson(input, contentType: "application/json", accept: "application/json")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentShouldBe("application/json", expectedJson);

            endpoints.PostJson(input, contentType: "application/json", accept: "text/xml,application/json")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentShouldBe("application/json", expectedJson);

            endpoints.PostJson(input, contentType: "text/json", accept: "text/xml,text/json")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentShouldBe("text/json", expectedJson);
        }

        [Test]
        public void uses_json_for_global_accept()
        {
            endpoints.PostJson(input, contentType: "text/json", accept: "*/*")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentShouldBe("text/json", expectedJson);

            endpoints.PostJson(input, contentType: "text/json", accept: "text/xml,*/*")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ContentShouldBe("text/json", expectedJson);
        }

        [Test]
        public void will_not_accept_xml_as_an_input()
        {
            endpoints.PostXml(input, accept: "*/*").StatusCodeShouldBe(HttpStatusCode.UnsupportedMediaType);
        }

        [Test]
        public void requesting_an_unsupported_media_type_returns_406()
        {
            endpoints.PostJson(input, accept: "text/xml").StatusCodeShouldBe(HttpStatusCode.NotAcceptable);
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
    }


    public class ConnegController
    {
        [SymmetricJson]
        public SymmetricJson post_send_symmetric(SymmetricJson message)
        {
            message.Name = "I was here";
            return message;
        }

        [AsymmetricJson]
        public AsymmetricJson post_send_asymmetric(AsymmetricJson message)
        {
            return message;
        }

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

    public class SymmetricJson : ConnegMessage
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }

    public class AsymmetricJson : ConnegMessage
    {
        public Guid Id { get; set; }
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