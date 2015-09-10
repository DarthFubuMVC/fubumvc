using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.ServiceBus.Monitoring;
using HtmlTags;
using Serenity.ServiceBus;
using StoryTeller;
using StoryTeller.Grammars.Tables;

namespace FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Monitoring
{
    public class MonitoringFixture : FubuTransportActFixture
    {
        private MonitoredNodeGroup _nodes;

        public MonitoringFixture()
        {
            Title = "Health Monitoring, Failover, and Task Assignment";

            this["Context"] = Embed<MonitoringSetupFixture>("If the nodes and tasks are");
        }

        protected override void setup()
        {
            _nodes = new MonitoredNodeGroup();
            Context.State.Store(_nodes);
        }

        protected override void teardown()
        {
            var messages = _nodes.LoggedEvents().ToArray();
            var table = new TableTag();
            table.AddHeaderRow(_ =>
            {
                _.Header("Node");
                _.Header("Subject");
                _.Header("Type");
                _.Header("Message");
            });

            messages.Each(message =>
            {
                table.AddBodyRow(_ =>
                {
                    _.Cell(message.NodeId);
                    _.Cell(message.Subject.ToString());
                    _.Cell(message.GetType().Name);
                    _.Cell(message.ToString());
                });
            });


            Context.Reporting.Log("Monitored Node Group", table.ToString());

            _nodes.Dispose();
        }

        [ExposeAsTable("If the task state is")]
        public void TaskStateIs(
            Uri Task,
            string Node,
            [SelectionValues(MonitoredNode.HealthyAndFunctional, MonitoredNode.TimesOutOnStartupOrHealthCheck,
                MonitoredNode.ThrowsExceptionOnStartupOrHealthCheck, MonitoredNode.IsInactive)] string State)
        {
            _nodes.SetTaskState(Task, Node, State);
        }

        [FormatAs("After the health checks run on all nodes")]
        public void AfterTheHealthChecksRunOnAllNodes()
        {
            _nodes.WaitForAllHealthChecks();

            waitForTheMessageProcessingToFinish();
        }

        [FormatAs("Task {task} was not reassigned")]
        public bool TaskWasNotReassigned(Uri task)
        {
            var events = _nodes.LoggedEvents().OfType<ReassigningTask>()
                .Where(x => x.Subject == task);

            if (events.Any())
            {
                StoryTellerAssert.Fail("Task {0} was reassigned: " + events.Select(x => x.ToString()).Join("\n"));
            }

            return true;
        }

        [FormatAs("After the health checks run on node {node}")]
        public void AfterTheHealthChecksRunOnNode(string node)
        {
            _nodes.WaitForHealthChecksOn(node);

            waitForTheMessageProcessingToFinish();
        }

        [FormatAs("Node {Node} drops offline")]
        public void NodeDropsOffline(string Node)
        {
            _nodes.ShutdownNode(Node);
        }

        public IGrammar TheTaskAssignmentsShouldBe()
        {
            return VerifySetOf(() => _nodes.AssignedTasks())
                .Titled("The task assignments should be")
                .MatchOn(x => x.Task, x => x.Node);
        }

        public IGrammar ThePersistedAssignmentsShouldBe()
        {
            return VerifySetOf(() => _nodes.PersistedTasks())
                .Titled("The persisted task assignments should be")
                .MatchOn(x => x.Task, x => x.Node);
        }

        public IGrammar ThePersistedNodesShouldBe()
        {
            return VerifySetOf(() => _nodes.GetPersistedNodes())
                .Titled("The persisted nodes should be")
                .MatchOn(x => x.Id, x => x.ControlChannel);
        }
    }
}