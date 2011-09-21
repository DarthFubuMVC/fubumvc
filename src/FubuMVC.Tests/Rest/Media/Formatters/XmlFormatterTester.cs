using FubuMVC.Core;
using FubuMVC.Core.Rest.Conneg;
using FubuMVC.Core.Rest.Media.Formatters;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Rest.Conneg
{
    [TestFixture]
    public class XmlFormatterTester 
    {
        private InMemoryStreamingData streamingData;
        private XmlFormatter theFormatter;

        [SetUp]
        public void SetUp()
        {
            streamingData = new InMemoryStreamingData();
            theFormatter = new XmlFormatter(streamingData);
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

            streamingData.RewindOutput();
            streamingData.CopyOutputToInputForTesting();

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