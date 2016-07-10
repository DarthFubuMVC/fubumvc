﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using StoryTeller;
using StoryTeller.Grammars.Tables;

namespace FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Monitoring
{
    public class MonitoringSetupFixture : Fixture
    {
        private MonitoredNodeGroup _nodes;

        public MonitoringSetupFixture()
        {
            Title = "If the nodes within a cluster are";
        }

        public override void SetUp()
        {
            _nodes = Context.State.Retrieve<MonitoredNodeGroup>();

            // Really important to do this!
            MonitoredNode.SubscriptionPersistence.ClearAll();
        }

        public override void TearDown()
        {
            _nodes.Startup();

            var nodes = MonitoredNode.SubscriptionPersistence.AllNodes().Select(x => x.ToString()).Join(", ");
            Debug.WriteLine($"The active transport nodes are {nodes}");
        }

        [FormatAs("The Health Monitoring job is enabled in all nodes")]
        public void HealthMonitoringIsEnabled()
        {
            _nodes.MonitoringEnabled = true;
        }


        [FormatAs("The Health Monitoring job is initially disabled in all nodes")]
        public void HealthMonitoringIsDisabled()
        {
            _nodes.MonitoringEnabled = false;
        }

        [ExposeAsTable("The nodes in this group are")]
        public void TheNodesAre([Header("Node Id")] string id, [Header("Incoming Uri")] Uri incoming)
        {
            _nodes.Add(id, incoming);
        }

        [ExposeAsTable("The permanent tasks are")]
        public void TheTasksAre([Header("Task Uri")] Uri task, [Header("Initial Assigned Node")] string node,
            [Header("Preferred Nodes in Order")] string[] nodes)
        {
            _nodes.AddTask(task, node, nodes);
        }
    }
}