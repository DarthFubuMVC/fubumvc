using FubuMVC.Media.Xml;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Media.Testing.Xml
{
    [TestFixture]
    public class XmlProjectionTester
    {
        [Test]
        public void create_a_projection()
        {
            var projection = new XmlProjection<Address>();
            projection.Value(x => x.Line1);
            projection.Value(x => x.Line2);
            projection.Value(x => x.City);

            var address = new Address{
                City = "Austin",
                Line1 = "1718 W 10th St",
                Line2 = "Unit A"
            };

            var document = projection.Write(address);

            document.OuterXml.ShouldEqual(
                "<Address><Line1>1718 W 10th St</Line1><Line2>Unit A</Line2><City>Austin</City></Address>");
        }

        [Test]
        public void root_by_default_is_derived_from_the_type_name()
        {
            var projection = new XmlProjection<Address>();
            projection.Root.ShouldEqual(typeof (Address).Name);
        }
    }
}