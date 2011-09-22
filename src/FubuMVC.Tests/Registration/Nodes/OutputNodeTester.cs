using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Nodes
{
    [TestFixture]
    public class OutputNodeTester
    {
        private OutputNode _node;
        private Type _behaviorType;

        [SetUp]
        public void SetUp()
        {
            _behaviorType = typeof(BasicBehavior);
            _node = OutputNode.For<BasicBehavior>();
        }

        [Test]
        public void for_instantiate_new_node()
        {
            _node.BehaviorType.ShouldEqual(_behaviorType);
        }

        [Test]
        public void description_returns_behavior_type_full_name()
        {
            _node.Description.ShouldEqual(_behaviorType.Name);
        }

        [Test]
        public void to_string_returns_description()
        {
            _node.ToString().ShouldEqual(_behaviorType.Name);
        }
    }
}