using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.Runtime.Formatters
{
    [TestFixture]
    public class XmlFormatterTester 
    {
        private InMemoryStreamingData streamingData;
        private XmlFormatter theFormatter;
        private InMemoryOutputWriter writer;
        private MockedFubuRequestContext context;

        [SetUp]
        public void SetUp()
        {
            streamingData = new InMemoryStreamingData();
            writer = new InMemoryOutputWriter();

            var container = new Container(x => {
                x.For<IHttpRequest>().Use(streamingData);
                x.For<IOutputWriter>().Use(writer);
                x.For<IFubuRequest>().Use(new InMemoryFubuRequest());
            });

            context = new MockedFubuRequestContext(container);

            theFormatter = new XmlFormatter();
        }

        [Test]
        public void read_value()
        {
            var xmlInput = new XmlFormatterModel(){
                FirstName = "Jeremy",
                LastName = "Miller"
            };

            streamingData.XmlInputIs(xmlInput);

            var xmlOutput = theFormatter.Read<XmlFormatterModel>(context);
            xmlOutput.ShouldNotBeTheSameAs(xmlInput);

            xmlOutput.FirstName.ShouldEqual(xmlInput.FirstName);
            xmlOutput.LastName.ShouldEqual(xmlInput.LastName);
        }

        [Test]
        public void write_value()
        {
            var xmlInput = new XmlFormatterModel()
            {
                FirstName = "Jeremy",
                LastName = "Miller"
            };

            theFormatter.Write(context, xmlInput, "text/xml");

            streamingData.CopyOutputToInputForTesting(writer.OutputStream());

            var xmlOutput = theFormatter.Read<XmlFormatterModel>(context);
            xmlOutput.ShouldNotBeTheSameAs(xmlInput);

            xmlOutput.FirstName.ShouldEqual(xmlInput.FirstName);
            xmlOutput.LastName.ShouldEqual(xmlInput.LastName);
        }
    }

    public class XmlFormatterModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}