using System.Diagnostics;
using System.Xml;
using System.Linq;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Media.Projections;
using FubuMVC.Media.Xml;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Media.Testing.Xml
{

    [TestFixture]
    public class XmlMediaWriterIntegratedTester
    {
        private InMemoryOutputWriter theOutput;
        private XmlMediaOptions theXmlMediaOptions;
        private XmlMediaDocument theDocument;
        private Address theAddress;

        [SetUp]
        public void SetUp()
        {
            var projection = new Projection<Address>(DisplayFormatting.RawValues);
            projection.Value(x => x.Address1);
            projection.Value(x => x.Address2);
            projection.Value(x => x.City);
            projection.Value(x => x.StateOrProvince).Name("State");

            theXmlMediaOptions = new XmlMediaOptions(){
                Root = "Address"
            };
            theDocument = new XmlMediaDocument(theXmlMediaOptions);

            var urls = new StubUrlRegistry();

            var linkSource = new LinksSource<Address>();
            linkSource.ToSubject().Rel("self");
            linkSource.To(a => new AddressAction("change")).Rel("change");
            linkSource.To(a => new AddressAction("delete")).Rel("delete");

            theOutput = new InMemoryOutputWriter();

            var media = new MediaWriter<Address>(theDocument, linkSource, urls, projection, null, theOutput);


            theAddress = new Address(){
                Address1 = "22 Cherry Lane",
                Address2 = "Apt A",
                City = "Austin",
                StateOrProvince = "Texas"
            };



            media.Write("text/plain", theAddress);

            Debug.WriteLine(theOutput);
        }

        [Test]
        public void should_write_the_output_mimetype()
        {
            theOutput.ContentType.ShouldEqual(theXmlMediaOptions.Mimetype);
        }

        [Test]
        public void the_contents_of_the_writer_should_equal_the_xml()
        {
            theOutput.ToString().Trim().ShouldEqual(theDocument.Document.OuterXml.Trim());
        }

        [Test]
        public void has_the_projection_elements()
        {
            var root = theDocument.Document.DocumentElement;
            root["Address1"].InnerText.ShouldEqual(theAddress.Address1);
            root["Address2"].InnerText.ShouldEqual(theAddress.Address2);
            root["City"].InnerText.ShouldEqual(theAddress.City);
            root["State"].InnerText.ShouldEqual(theAddress.StateOrProvince);
        }

        [Test]
        public void has_the_links()
        {
            var xmlNodeList = theDocument.Document.DocumentElement.SelectNodes("link");
            var links = new System.Collections.Generic.List<XmlElement>();
            foreach (XmlElement element in xmlNodeList)
            {
                links.Add(element);
            }

            links.Select(x => x.GetAttribute("href"))
                .ShouldHaveTheSameElementsAs("url for FubuMVC.Media.Testing.Xml.Address", "url for Action/change", "url for Action/delete");
            
        }
    }

    public class AddressAction
    {
        private readonly string _name;

        public AddressAction(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public override string ToString()
        {
            return string.Format("Action/{0}", _name);
        }
    }

    
}