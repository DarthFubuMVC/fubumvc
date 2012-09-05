using System.Collections.Generic;
using FubuMVC.Core;
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
        private BehaviorChain theChain;

        [SetUp]
        public void SetUp()
        {
            source1 = new LambdaConfigurationAction(g => { });
            theLog = new ConfigLog(null);
            theChain = new BehaviorChain();

            node = new TracedNode();
            node.Trace("something");
            node.Trace("else");

            node2 = new TracedNode();

            theConfigSource = theLog.StartSource(new FubuRegistry(), source1);
            theLog.RecordEvents(theChain, node);
            theLog.RecordEvents(theChain, node2);
        }

        [Test]
        public void places_the_chain_on_the_NodeEvent()
        {
            theConfigSource.Events.Each(x => x.Chain.ShouldBeTheSameAs(theChain));
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

    }
}