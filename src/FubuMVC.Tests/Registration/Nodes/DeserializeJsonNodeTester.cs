using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Nodes
{
    [TestFixture]
    public class DeserializeJsonNodeTester
    {
        private DeserializeJsonNode node;
        private Type _messageType = typeof (JsonMessageClass);

        [SetUp]
        public void SetUp()
        {
            node = new DeserializeJsonNode(_messageType);
        }

        [Test]
        public void the_node_should_be_a_wrapper()
        {
            node.Category.ShouldEqual(BehaviorCategory.Wrapper);
        }

        [Test]
        public void should_build_an_object_def_for_a_json_deserialization_behavior()
        {
            node.As<IContainerModel>().ToObjectDef().Type.ShouldEqual(typeof(DeserializeJsonBehavior<JsonMessageClass>));
        }

        [Test]
        public void should_tell_if_equal()
        {
            node.Equals(node).ShouldBeTrue();
            node.Equals(null).ShouldBeFalse();
            node.Equals((object)null).ShouldBeFalse();
            node.Equals((object)node).ShouldBeTrue();
            node.Equals("").ShouldBeFalse();
            node.Equals((object)new DeserializeJsonNode(_messageType)).ShouldBeTrue();
        }

        [Test]
        public void should_return_message_type_hash_code()
        {
            node.GetHashCode().ShouldEqual(_messageType.GetHashCode());
        }

        [Test]
        public void should_get_message_type()
        {
            node.MessageType.ShouldEqual(_messageType);
        }
    }

    public class JsonMessageClass{}
}