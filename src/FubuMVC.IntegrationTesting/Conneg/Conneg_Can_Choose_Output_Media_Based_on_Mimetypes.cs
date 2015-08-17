using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using FubuMVC.Core.Json;
using FubuMVC.Core.Runtime;
using Newtonsoft.Json;
using Shouldly;
using HtmlTags;
using NUnit.Framework;
using Formatting = System.Xml.Formatting;

namespace FubuMVC.IntegrationTesting.Conneg
{
    [Obsolete("Remove this after the ST specs are done")]
    [TestFixture]
    public class conneg_with_endpoint_that_accepts_all_formatters_and_form_posts
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

            expectedJson = TestHost.Service<IJsonSerializer>().Serialize(input);

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
            TestHost.Scenario(_ =>
            {
                
                _.Post.Json(input).Accepts("random/format");
                _.StatusCodeShouldBe(HttpStatusCode.NotAcceptable);
            });
        }

        [Test]
        public void send_no_accept_header_but_treat_like_anything_is_accepted()
        {
            TestHost.Scenario(_ =>
            {
                _.Post.Json(input);
                _.ContentTypeShouldBe(MimeType.Json);
                _.Response.Body.ReadAsJson<XmlJsonHtmlMessage>()
                    .Id.ShouldBe(input.Id);
            });
        }

        [Test]
        public void send_json_expecting_json()
        {
            TestHost.Scenario(_ =>
            {
                _.Post.Json(input).ContentType("text/json").Accepts("text/json");
                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("text/json");
                _.ContentShouldBe(expectedJson);
            });

            TestHost.Scenario(_ =>
            {
                _.Post.Json(input).ContentType("application/json").Accepts("application/json");
                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("application/json");
                _.ContentShouldBe(expectedJson);
            });

            TestHost.Scenario(_ =>
            {
                _.Post.Json(input).ContentType("application/json").Accepts("application/json,text/xml");
                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("application/json");
                _.ContentShouldBe(expectedJson);
            });

            TestHost.Scenario(_ =>
            {
                _.Post.Json(input).ContentType("text/json").Accepts("text/json,text/xml");
                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("text/json");
                _.ContentShouldBe(expectedJson);
            });

        }

        [Test]
        public void send_the_request_as_http_form_expect_json_back()
        {
            TestHost.Scenario(_ =>
            {
                _.Post.FormData(input).Accepts("text/json");
                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("text/json");
                _.ContentShouldBe(expectedJson);
            });
            
            TestHost.Scenario(_ =>
            {
                _.Post.FormData(input).Accepts("application/json");
                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("application/json");
                _.ContentShouldBe(expectedJson);
            });

        }

        [Test]
        public void send_the_request_as_http_form_expect_xml_back()
        {
            TestHost.Scenario(_ =>
            {
                _.Post.FormData(input).Accepts("text/xml");
                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("text/xml");
                _.ContentShouldBe(expectedXml);
            });

            TestHost.Scenario(_ =>
            {
                _.Post.FormData(input).Accepts("application/xml");
                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("application/xml");
                _.ContentShouldBe(expectedXml);
            });
        }

        [Test]
        public void uses_json_for_global_accept()
        {
            TestHost.Scenario(_ =>
            {
                _.Post.Json(input).ContentType("text/json").Accepts("*/*");
                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("application/json");
                _.ContentShouldBe(expectedJson);
            });

            TestHost.Scenario(_ =>
            {
                _.Post.Json(input).ContentType("text/json").Accepts("something/weird,*/*");
                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("application/json");
                _.ContentShouldBe(expectedJson);
            });
        }

        [Test]
        public void will_accept_xml_as_an_input()
        {
            TestHost.Scenario(_ =>
            {
                _.Post.Xml(input);
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