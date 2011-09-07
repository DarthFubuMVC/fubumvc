using System;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration.Nodes
{
     [TestFixture]
    public class ConditionalNodeTester 
    {
        private BehaviorNode _behaviorNode;
        private ConditionalNode _node;

        [SetUp]
        public void Setup()
        {
            _behaviorNode = new FakeNode();
            _node = new ConditionalNode(_behaviorNode, () => true);
        }

        [Test]
        public void top_level_object_def_should_be_an_invoker()
        {
            ToObjectDef().Type.ShouldEqual(typeof (ConditionalBehaviorInvoker));
        }

        private ObjectDef ToObjectDef()
        {
            return _node.As<IContainerModel>().ToObjectDef();
        }

        [Test]
        public void should_contain_inner_action_behavior_dependecy_of_I_condition_behavior()
        {
            ToObjectDef().DependencyFor<IConditionalBehavior>().ShouldNotBeNull();
        }

        [Test]
        public void should_contain_inner_action_behavior_dependecy_of_should_have_action_behavior_dependency()
        {
            ObjectDef objectDef = ToObjectDef().DependencyFor<IConditionalBehavior>()
                .As<ConfiguredDependency>().Definition;

            objectDef.DependencyFor<IActionBehavior>().ShouldNotBeNull();
        }

        [Test]
        public void should_contain_inner_action_behavior_dependecy_of_should_have_condition_dependency()
        {
            ObjectDef objectDef = ToObjectDef().DependencyFor<IConditionalBehavior>()
                .As<ConfiguredDependency>().Definition;

            objectDef.DependencyFor<Func<bool>>().ShouldNotBeNull();
        }
    }
     [TestFixture]
    public class ConditionalNodeCtorTester
    {
        [Test]
        public void should_throw()
        {
            typeof (FubuAssertionException)
                .ShouldBeThrownBy(() =>
                                      {
                                          new ConditionalNode(new FakeNode(), typeof (FakeNode));
                                      });
        }

      
    }


      [TestFixture]
     public class ConditionalNodeOfTTester
    {
        private BehaviorNode _behaviorNode;
        private ConditionalNode<IRequestData> _node;

        [SetUp]
        public void Setup()
        {
            _behaviorNode = new FakeNode();
            _node = new ConditionalNode<IRequestData>(_behaviorNode, x => true);
        }

        [Test]
        public void top_level_object_def_should_be_an_invoker()
        {
            ToObjectDef().Type.ShouldEqual(typeof(ConditionalBehaviorInvoker));
        }

        private ObjectDef ToObjectDef()
        {
            return _node.As<IContainerModel>().ToObjectDef();
        }

        [Test]
        public void should_contain_inner_action_behavior_dependecy_of_I_condition_behavior()
        {
            ToObjectDef().DependencyFor<IConditionalBehavior>().ShouldNotBeNull();
        }
        
        [Test]
        public void should_contain_inner_action_behavior_dependecy_of_should_have_action_behavior_dependency()
        {
            ObjectDef objectDef = ToObjectDef().DependencyFor<IConditionalBehavior>()
                .As<ConfiguredDependency>().Definition;

            objectDef.DependencyFor<IActionBehavior>().ShouldNotBeNull();
        }

        [Test]
        public void should_contain_inner_action_behavior_dependecy_of_should_have_condition_dependency()
        {
            ObjectDef objectDef = ToObjectDef().DependencyFor<IConditionalBehavior>()
                .As<ConfiguredDependency>().Definition;

            objectDef.DependencyFor<Func<IRequestData,bool>>().ShouldNotBeNull();
        }

    }

    public class FakeNode : BehaviorNode
    {
        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Call; }
        }

        protected override ObjectDef buildObjectDef()
        {
            return new ObjectDef(typeof(FakeBehavior));
        }
    }
}