using FubuCore;
using FubuCore.Formatting;
using FubuCore.Reflection;
using FubuMVC.Media.Projections;
using FubuMVC.Media.Xml;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Media.Testing.Xml
{
    [TestFixture]
    public class ProjectionTester
    {
        private Address anAddress;
        private XmlAttCentricMediaNode aNode;
        private SimpleValues<Address> aTarget;

        [SetUp]
        public void SetUp()
        {
            anAddress = new Address{
                Line1 = "22 Cherry Lane",
                City = "Austin",
                State = "TX",
                ZipCode = "78703"
            };

            aTarget = new SimpleValues<Address>(anAddress);
            aNode = XmlAttCentricMediaNode.ForRoot("root");
        }

        [Test]
        public void write_node_with_inline_writer()
        {
            var projection = new Projection<Address>(DisplayFormatting.RawValues);
            projection.WriteWith((context, node) =>
            {
                node.AddChild("address").SetAttribute("description", "I was here");
            });

            projection.As<IProjection<Address>>().Write(new ProjectionContext<Address>(null, aTarget), aNode);

            aNode.Element.InnerXml.ShouldEqual("<address description=\"I was here\" />");
        }

        [Test]
        public void write_a_node_with_multiple_properties()
        {
            var projection = new Projection<Address>(DisplayFormatting.RawValues);
            projection.Value(x => x.Line1);
            projection.Value(x => x.City);
            projection.Value(x => x.State);
            projection.Value(x => x.ZipCode);


            projection.As<IProjection<Address>>().Write(new ProjectionContext<Address>(null, aTarget), aNode);

            aNode.Element.GetAttribute("Line1").ShouldEqual(anAddress.Line1);
            aNode.Element.GetAttribute("City").ShouldEqual(anAddress.City);
            aNode.Element.GetAttribute("State").ShouldEqual(anAddress.State);
            aNode.Element.GetAttribute("ZipCode").ShouldEqual(anAddress.ZipCode);
        }

        [Test]
        public void project_with_the_for_attribute_method()
        {
            var projection = new Projection<Address>(DisplayFormatting.RawValues);

            projection.ForAttribute("anything").Use(context => "aaa" + context.ValueFor(x => x.Line1));

            projection.As<IProjection<Address>>().Write(new ProjectionContext<Address>(null, aTarget), aNode);

            aNode.Element.GetAttribute("anything").ShouldEqual("aaa" + anAddress.Line1);
        }

        [Test]
        public void project_with_the_for_attribute_and_func_to_massage_the_data()
        {
            var projection = new Projection<Address>(DisplayFormatting.RawValues);

            projection.ForAttribute("anything").ValueFrom(x => x.Line1).Use(value => "aaa" + value);

            projection.As<IProjection<Address>>().Write(new ProjectionContext<Address>(null, aTarget), aNode);

            aNode.Element.GetAttribute("anything").ShouldEqual("aaa" + anAddress.Line1);
        }

        [Test]
        public void project_the_property_with_formatting_thru_display_formatter()
        {
            var projection = new Projection<Address>(DisplayFormatting.UseDisplayFormatting);
            projection.Value(x => x.Line1);

            var services = MockRepository.GenerateMock<IServiceLocator>();
            var formatter = MockRepository.GenerateMock<IDisplayFormatter>();
            services.Stub(x => x.GetInstance<IDisplayFormatter>()).Return(formatter);

            var accessor = ReflectionHelper.GetAccessor<Address>(x => x.Line1);
            var theFormattedValue = "formatted value";

            formatter.Stub(x => x.GetDisplayForValue(accessor, anAddress.Line1)).Return(theFormattedValue);

            var node = new DictionaryMediaNode();
            projection.As<IProjection<Address>>().Write(new ProjectionContext<Address>(services, aTarget), node);
            node.Values["Line1"].ShouldEqual(theFormattedValue);
        }
    }



    public class Address
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string StateOrProvince { get; set; }
    }
}