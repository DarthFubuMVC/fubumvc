using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Nodes
{
    public class ConditionalNodeTester
    {
        private ConditionalBehavior toBehavior(ConditionalNode node)
        {
            var container = StructureMapContainerFacility.GetBasicFubuContainer();
            return
                container.GetInstance<ConditionalBehavior>(
                    new ObjectDefInstance(node.As<IContainerModel>().ToObjectDef()));
        }

        [Test]
        public void should_execute()
        {
            var node = new ConditionalNode(() => true);

            var behavior = toBehavior(node);

            behavior.ShouldExecute().ShouldBeTrue();
        }


        [Test]
        public void should_not_execute()
        {
            var node = new ConditionalNode(() => false);

            var behavior = toBehavior(node);

            behavior.ShouldExecute().ShouldBeFalse();
        }
    
    }
}