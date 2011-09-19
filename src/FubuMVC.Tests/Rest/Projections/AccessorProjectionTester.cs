using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Rest.Media.Projections;
using FubuMVC.Core.Rest.Media.Projections.Xml;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Rest.Projections
{
    [TestFixture]
    public class AccessorProjectionTester
    {
        private AccessorProjection<ValueTarget> theAccessorProjection;
        private SimpleValues<ValueTarget> _theValues;
        private XmlAttCentricMediaNode theMediaNode;

        [SetUp]
        public void SetUp()
        {
            theAccessorProjection = AccessorProjection<ValueTarget>.For(x => x.Age);
            _theValues = new SimpleValues<ValueTarget>(new ValueTarget
            {
                Age = 37
            });

            theMediaNode = XmlAttCentricMediaNode.ForRoot("root");
        }

        [Test]
        public void project_the_property_with_default_node_name()
        {
            theAccessorProjection.WriteValue(_theValues, theMediaNode);

            theMediaNode.Element.GetAttribute("Age").ShouldEqual("37");
        }

        [Test]
        public void project_the_property_with_a_different_node_name()
        {
            theAccessorProjection.Name("CurrentAge");

            theAccessorProjection.WriteValue(_theValues, theMediaNode);

            theMediaNode.Element.GetAttribute("CurrentAge").ShouldEqual("37");
        }
    }

    public class ValueTarget
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}