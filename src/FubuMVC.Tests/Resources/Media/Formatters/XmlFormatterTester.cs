using FubuMVC.Core.Resources.Media.Formatters;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Resources.Media.Formatters
{
    [TestFixture]
    public class XmlFormatterTester 
    {
        private InMemoryStreamingData streamingData;
        private XmlFormatter theFormatter;
        private InMemoryOutputWriter writer;

        [SetUp]
        public void SetUp()
        {
            streamingData = new InMemoryStreamingData();
            writer = new InMemoryOutputWriter();
            
            theFormatter = new XmlFormatter(streamingData, writer);
        }

        [Test]
        public void read_value()
        {
            var xmlInput = new XmlFormatterModel(){
                FirstName = "Jeremy",
                LastName = "Miller"
            };

            streamingData.XmlInputIs(xmlInput);

            var xmlOutput = theFormatter.Read<XmlFormatterModel>();
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

            theFormatter.Write(xmlInput, "text/xml");

            streamingData.CopyOutputToInputForTesting(writer.OutputStream());

            var xmlOutput = theFormatter.Read<XmlFormatterModel>();
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