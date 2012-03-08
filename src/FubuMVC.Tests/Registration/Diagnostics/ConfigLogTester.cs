using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Diagnostics
{

    [TestFixture]
    public class when_recording_events_from_a_traced_node
    {
        private LambdaConfigurationAction source1;
        private ConfigLog theLog;
        private ITracedModel node;
        private ITracedModel node2;
        private ConfigSource theConfigSource;

        [SetUp]
        public void SetUp()
        {
            source1 = new LambdaConfigurationAction(g => { });
            theLog = new ConfigLog();


            node = new TracedNode();
            node.Trace("something");
            node.Trace("else");

            node2 = new TracedNode();

            theConfigSource = theLog.StartSource(source1);
            theLog.RecordEvents(node);
            theLog.RecordEvents(node2);
        }

        [Test]
        public void theConfigSource_has_all_the_events()
        {
            theConfigSource.Events.Count.ShouldEqual(4);
        }

        [Test]
        public void emptied_the_node_of_all_unstaged_events()
        {
            node.StagedEvents.Any().ShouldBeFalse();
            node2.StagedEvents.Any().ShouldBeFalse();
        }

        [Test]
        public void can_retrieve_all_the_events_for_the_first_node()
        {
            var nodeEvents = theLog.EventsBySubject(node);
            nodeEvents.ShouldHaveCount(3);
            nodeEvents.ElementAt(0).ShouldBeOfType<Created>();
            nodeEvents.ElementAt(1).ShouldBeOfType<Traced>();
            nodeEvents.ElementAt(2).ShouldBeOfType<Traced>();
        }

        [Test]
        public void associates_the_source_with_all_the_events()
        {
            theLog.EventsBySubject(node).Each(x => x.Source.Action.ShouldBeTheSameAs(source1));
            theLog.EventsBySubject(node2).Each(x => x.Source.Action.ShouldBeTheSameAs(source1));
        }

        [Test]
        public void can_retrieve_events_for_the_second_node()
        {
            theLog.EventsBySubject(node2).Single().ShouldBeOfType<Created>()
                .Subject.ShouldBeTheSameAs(node2);
        }
    }
}