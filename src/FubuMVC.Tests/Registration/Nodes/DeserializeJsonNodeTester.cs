using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Nodes
{
    [TestFixture]
    public class DeserializeJsonNodeTester
    {
        private DeserializeJsonNode node;

        [SetUp]
        public void SetUp()
        {
            node = new DeserializeJsonNode(typeof(JsonMessageClass));
        }

        [Test]
        public void the_node_should_be_a_wrapper()
        {
            node.Category.ShouldEqual(BehaviorCategory.Wrapper);
        }

        [Test]
        public void should_build_an_object_def_for_a_json_deserialization_behavior()
        {
            node.ToObjectDef().Type.ShouldEqual(typeof (DeserializeJsonBehavior<JsonMessageClass>));
        }
    }

    public class JsonMessageClass{}
}