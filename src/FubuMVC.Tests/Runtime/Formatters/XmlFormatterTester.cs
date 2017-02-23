using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;
using Shouldly;
using Xunit;
using StructureMap;

namespace FubuMVC.Tests.Runtime.Formatters
{
    
    public class XmlFormatterTester 
    {
        private OwinHttpRequest theRequest;
        private XmlFormatter theFormatter;
        private InMemoryOutputWriter writer;
        private MockedFubuRequestContext context;

        public XmlFormatterTester()
        {
            theRequest = OwinHttpRequest.ForTesting();
            writer = new InMemoryOutputWriter();

            var container = new Container(x => {
                x.For<IHttpRequest>().Use(theRequest);
                x.For<IOutputWriter>().Use(writer);
                x.For<IFubuRequest>().Use(new InMemoryFubuRequest());
            });

            context = new MockedFubuRequestContext(container);

            theFormatter = new XmlFormatter();
        }

        [Fact]
        public void read_value()
        {
            var xmlInput = new XmlFormatterModel(){
                FirstName = "Jeremy",
                LastName = "Miller"
            };

            theRequest.Body.XmlInputIs(xmlInput);

            var xmlOutput = theFormatter.Read<XmlFormatterModel>(context);
            xmlOutput.ShouldNotBeTheSameAs(xmlInput);

            xmlOutput.FirstName.ShouldBe(xmlInput.FirstName);
            xmlOutput.LastName.ShouldBe(xmlInput.LastName);
        }

        [Fact]
        public void write_value()
        {
            var xmlInput = new XmlFormatterModel()
            {
                FirstName = "Jeremy",
                LastName = "Miller"
            };

            theFormatter.Write(context, xmlInput, "text/xml");

            theRequest.Body.ReplaceBody(writer.OutputStream());

            var xmlOutput = theFormatter.Read<XmlFormatterModel>(context);
            xmlOutput.ShouldNotBeTheSameAs(xmlInput);

            xmlOutput.FirstName.ShouldBe(xmlInput.FirstName);
            xmlOutput.LastName.ShouldBe(xmlInput.LastName);
        }
    }

    public class XmlFormatterModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}