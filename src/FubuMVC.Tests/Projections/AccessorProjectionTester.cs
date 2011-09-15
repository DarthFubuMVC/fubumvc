using FubuMVC.Core.Projections;
using FubuMVC.Core.Projections.Xml;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Projections
{
    [TestFixture]
    public class AccessorProjectionTester
    {
        private AccessorProjection theAccessorProjection;
        private SimpleProjectionTarget theProjectionTarget;
        private XmlMediaNode theMediaNode;

        [SetUp]
        public void SetUp()
        {
            theAccessorProjection = AccessorProjection.For<ValueTarget>(x => x.Age);
            theProjectionTarget = new SimpleProjectionTarget(new ValueTarget{
                Age = 37
            });

            theMediaNode = XmlMediaNode.ForRoot("root");
        }

        [Test]
        public void project_the_property_with_default_node_name()
        {
            theAccessorProjection.WriteValue(theProjectionTarget, theMediaNode);

            theMediaNode.Element.GetAttribute("Age").ShouldEqual("37");
        }

        [Test]
        public void project_the_property_with_a_different_node_name()
        {
            theAccessorProjection.Name("CurrentAge");

            theAccessorProjection.WriteValue(theProjectionTarget, theMediaNode);

            theMediaNode.Element.GetAttribute("CurrentAge").ShouldEqual("37");
        }
    }

    public class ValueTarget
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}