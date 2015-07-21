using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Subscriptions;
using HtmlTags;

namespace FubuMVC.Core.ServiceBus.Diagnostics.Visualization
{

    public class TasksFubuDiagnostics
    {
        private readonly ISubscriptionRepository _repository;
        private readonly IPersistentTaskController _tasks;

        public TasksFubuDiagnostics(ISubscriptionRepository repository, IPersistentTaskController tasks)
        {
            _repository = repository;
            _tasks = tasks;
        }

        [Description("Tasks:Permanent Tasks")]
        public HtmlTag get_tasks()
        {
            var peers = _repository.FindPeers();
            var cache = new Cache<Uri, TransportNode>();
            peers.Each(peer => peer.OwnedTasks.Each(x => cache[x] = peer));

            
            var tag = new HtmlTag("div");
            tag.Add("h1").Text("Task Assignements");


            var table = new TableTag();
            tag.Append(table);
            
            table.AddClass("table");

            table.AddHeaderRow(row => {
                row.Header("Task");
                row.Header("Assigned to");
                row.Header("Control Channel");
            });

            var tasks = _tasks.PermanentTasks().Union(_tasks.ActiveTasks()).ToArray();
            tasks.Each(uri => {
                table.AddBodyRow(row => addRow(row, uri, cache));
            });

            return tag;
        }

        private void addRow(TableRowTag row, Uri uri, Cache<Uri, TransportNode> peers)
        {
            row.Cell(uri.ToString());
            if (peers.Has(uri))
            {
                var peer = peers[uri];
                row.Cell(peer.Id);
                row.Cell(peer.ControlChannel.ToString());
            }
            else
            {
                row.Cell("None");
                row.Cell();
            }
        }
    }
}