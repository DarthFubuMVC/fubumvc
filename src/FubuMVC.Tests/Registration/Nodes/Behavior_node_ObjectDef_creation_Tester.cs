using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Registration.Nodes
{
    
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

        [Fact]
        public void creating_an_object_def_for_no_tracing()
        {
            var node = new Wrapper(typeof (SimpleBehavior));
            node.As<IContainerModel>().ToInstance()
                .ReturnedType.ShouldBe(typeof (SimpleBehavior));
        }
    }
}