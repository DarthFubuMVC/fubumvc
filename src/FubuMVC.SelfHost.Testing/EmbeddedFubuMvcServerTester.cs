using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;
using FubuMVC.StructureMap;
using StructureMap;

namespace FubuMVC.SelfHost.Testing
{
    [TestFixture]
    public class EmbeddedFubuMvcServerTester
    {
        private EmbeddedFubuMvcServer _application;

        [TestFixtureSetUp]
        public void Setup()
        {
            _application = FubuApplication.For<HarnessRegistry>().StructureMap(new Container()).RunEmbedded();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _application.Dispose();
        }

        [Test]
        public void read_and_write_json()
        {
            var message = new Message
            {
                Color = "Blue",
                Direction = "East"
            };

            var response = _application.Endpoints.PostJson(message, contentType: "text/json", accept: "text/json");

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

            var response = _application.Endpoints.PostXml(message, contentType: "text/xml", accept: "text/xml");

            response.StatusCodeShouldBe(HttpStatusCode.OK);
            response.ContentType.ShouldEqual("text/xml");

            var serializer = new XmlSerializer(typeof(Message));
            serializer.Deserialize(new XmlTextReader(new StringReader(response.ReadAsText()))).ShouldEqual(message);
        }
    }
}