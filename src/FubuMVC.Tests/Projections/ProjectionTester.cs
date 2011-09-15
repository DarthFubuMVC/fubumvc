using FubuCore;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Projections.Xml;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Projections
{
    [TestFixture]
    public class ProjectionTester
    {
        private Address anAddress;
        private XmlAttCentricMediaNode aNode;
        private SimpleValueSource<Address> aTarget;

        [SetUp]
        public void SetUp()
        {
            anAddress = new Address{
                Line1 = "22 Cherry Lane",
                City = "Austin",
                State = "TX",
                ZipCode = "78703"
            };

            aTarget = new SimpleValueSource<Address>(anAddress);
            aNode = XmlAttCentricMediaNode.ForRoot("root");
        }

        [Test]
        public void write_a_node_with_multiple_properties()
        {
            var projection = new Projection<Address>();
            projection.Value(x => x.Line1);
            projection.Value(x => x.City);
            projection.Value(x => x.State);
            projection.Value(x => x.ZipCode);

            
            projection.As<IValueProjection<Address>>().WriteValue(aTarget, aNode);

            aNode.Element.GetAttribute("Line1").ShouldEqual(anAddress.Line1);
            aNode.Element.GetAttribute("City").ShouldEqual(anAddress.City);
            aNode.Element.GetAttribute("State").ShouldEqual(anAddress.State);
            aNode.Element.GetAttribute("ZipCode").ShouldEqual(anAddress.ZipCode);
        }
    }



    public class Address
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }
}