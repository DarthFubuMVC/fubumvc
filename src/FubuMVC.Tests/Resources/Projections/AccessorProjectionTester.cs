using FubuMVC.Core.Resources.Media;
using FubuMVC.Core.Resources.Media.Projections;
using FubuMVC.Core.Resources.Media.Xml;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Resources.Projections
{
    [TestFixture]
    public class AccessorProjectionTester
    {
        private AccessorProjection<ValueTarget, int> theAccessorProjection;
        private SimpleValues<ValueTarget> _theValues;
        private XmlAttCentricMediaNode theMediaNode;

        [SetUp]
        public void SetUp()
        {
            theAccessorProjection = AccessorProjection<ValueTarget, int>.For(x => x.Age);
            _theValues = new SimpleValues<ValueTarget>(new ValueTarget
            {
                Age = 37
            });

            theMediaNode = XmlAttCentricMediaNode.ForRoot("root");
        }

        [Test]
        public void project_the_property_with_default_node_name()
        {
            theAccessorProjection.WriteValue(new ProjectionContext<ValueTarget>(null, _theValues), theMediaNode);

            theMediaNode.Element.GetAttribute("Age").ShouldEqual("37");
        }

        [Test]
        public void project_the_property_with_a_different_node_name()
        {
            theAccessorProjection.Name("CurrentAge");

            theAccessorProjection.WriteValue(new ProjectionContext<ValueTarget>(null, _theValues), theMediaNode);

            theMediaNode.Element.GetAttribute("CurrentAge").ShouldEqual("37");
        }

        [Test]
        public void project_the_property_with_formatting()
        {
            theAccessorProjection.FormattedBy(age => "*" + age + "*");
            theAccessorProjection.WriteValue(new ProjectionContext<ValueTarget>(null, _theValues), theMediaNode);

            theMediaNode.Element.GetAttribute("Age").ShouldEqual("*37*");
        }
    }

    public class ValueTarget
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}