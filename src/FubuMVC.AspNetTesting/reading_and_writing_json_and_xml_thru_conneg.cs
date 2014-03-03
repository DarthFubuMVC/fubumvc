using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using AspNetApplication;
using FubuMVC.IntegrationTesting;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.AspNetTesting
{
    [TestFixture]
    public class reading_and_writing_json_and_xml_thru_conneg
    {
        [Test]
        public void read_and_write_json()
        {
            var message = new Message
            {
                Color = "Blue",
                Direction = "East"
            };

            var response = TestApplication.Endpoints.PostJson(message, contentType: "text/json", accept: "text/json");

            response.StatusCodeShouldBe(HttpStatusCode.OK);
            response.ContentType.ShouldEqual("text/json");

            response.ReadAsJson<Message>().ShouldEqual(message);
        }

        [Test]
        public void read_and_write_xml()
        {
            var message = new Message
            {
                Color = "Blue",
                Direction = "East"
            };

            var response = TestApplication.Endpoints.PostXml(message, contentType: "text/xml", accept: "text/xml");

            response.StatusCodeShouldBe(HttpStatusCode.OK);
            response.ContentType.ShouldEqual("text/xml");

            var serializer = new XmlSerializer(typeof (Message));
            serializer.Deserialize(new XmlTextReader(new StringReader(response.ReadAsText()))).ShouldEqual(message);
        }
    }
}