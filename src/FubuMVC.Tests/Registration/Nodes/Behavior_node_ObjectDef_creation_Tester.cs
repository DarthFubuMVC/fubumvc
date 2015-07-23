using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Nodes
{
    [TestFixture]
    public class Behavior_node_ObjectDef_creation_Tester
    {
        public class SimpleBehavior : IActionBehavior
        {
            public void Invoke()
            {
            }

            public void InvokePartial()
            {
            }
        }

        public class DifferentBehavior : SimpleBehavior
        {
        }

        [Test]
        public void creating_an_object_def_for_no_tracing()
        {
            var node = new Wrapper(typeof (SimpleBehavior));
            node.As<IContainerModel>().ToInstance()
                .ReturnedType.ShouldBe(typeof (SimpleBehavior));
        }
    }
}