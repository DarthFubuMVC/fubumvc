using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Registration.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Security;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration.Diagnostics
{
    [TestFixture]
    public class BehaviorNodeTracingMechanicsTester
    {
        private ITracedModel theTracedNode;
        private AuthorizationNode theNode;

        [SetUp]
        public void SetUp()
        {
            theNode = new AuthorizationNode();
            theTracedNode = theNode.As<ITracedModel>();
        }

        [Test]
        public void just_after_being_created_there_should_be_a_Created_event()
        {
            theTracedNode.StagedEvents.Single().ShouldBeOfType<Created>();
        }

        [Test]
        public void trace_with_a_string_creates_a_new_trace_event()
        {
            theTracedNode.Trace("some text");
            theTracedNode.StagedEvents.Last().ShouldBeOfType<Traced>()
                .Text.ShouldEqual("some text");
        }

        [Test]
        public void recording_events_removes_the_events_from_the_node_and_calls_back()
        {
            var list = new List<NodeEvent>();

            theTracedNode.RecordEvents(list.Add);

            list.Single().ShouldBeOfType<Created>();

            theTracedNode.StagedEvents.Any().ShouldBeFalse();

            theTracedNode.Trace("something");

            theTracedNode.RecordEvents(list.Add);

            list.Last().ShouldBeOfType<Traced>().Text.ShouldEqual("something");

        }

        [Test]
        public void add_condition_with_description()
        {
            theNode.Condition(() => true, "wacky");

            theTracedNode.StagedEvents.Last().ShouldBeOfType<ConditionAdded>()
                .Description.ShouldEqual("wacky");
        
            
        }

        [Test]
        public void add_condition_by_function_against_service()
        {
            theNode.ConditionByService<Something>(x => true);

            theTracedNode.StagedEvents.Last().ShouldBeOfType<ConditionAdded>()
                .Description.ShouldEqual("By Service:  Func<Something, bool>");
        }

        [Test]
        public void add_condition_by_function_against_model()
        {
            theNode.ConditionByModel<Something>(x => true);

            theTracedNode.StagedEvents.Last().ShouldBeOfType<ConditionAdded>()
                .Description.ShouldEqual("By Model:  Func<Something, bool>");
        }

        [Test]
        public void add_condition_by_type()
        {
            theNode.Condition<SomethingCondition>();

            theTracedNode.StagedEvents.Last().ShouldBeOfType<ConditionAdded>()
                .Type.ShouldEqual(typeof (SomethingCondition));
        }

        [Test]
        public void the_remove_method_puts_a_node_removed_event_on_the_chain()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(new SimpleNode());

            var nodeToBeRemoved = new SimpleNode();
            chain.AddToEnd(nodeToBeRemoved);
            
            chain.AddToEnd(new SimpleNode());
            chain.AddToEnd(new SimpleNode());

            nodeToBeRemoved.Remove();

            chain.As<ITracedModel>().StagedEvents.Last()
                .ShouldEqual(new NodeRemoved(nodeToBeRemoved));
        }

        [Test]
        public void the_replace_method_puts_a_node_replaced_event_on_the_chain()
        {
            var original = new SimpleNode();
            var newNode = new SimpleNode();

            var chain = new BehaviorChain();
            chain.AddToEnd(original);

            original.ReplaceWith(newNode);

            chain.As<ITracedModel>().StagedEvents.Last()
                .ShouldEqual(new NodeReplaced(original, newNode));
        }

        public class SomethingCondition : IConditional
        {
            public bool ShouldExecute()
            {
                throw new NotImplementedException();
            }
        }

        public class Something{}

        public class SimpleNode : BehaviorNode
        {
            public override BehaviorCategory Category
            {
                get { throw new NotImplementedException(); }
            }

            protected override ObjectDef buildObjectDef()
            {
                throw new NotImplementedException();
            }
        }
    }
}