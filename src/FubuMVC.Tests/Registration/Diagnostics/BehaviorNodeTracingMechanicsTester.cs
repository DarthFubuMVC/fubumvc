using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Registration.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
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
    }
}