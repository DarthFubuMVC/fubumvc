using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.Services.Messaging;

namespace FubuMVC.Core.ServiceBus.Diagnostics
{
    public class MessagingSession : IMessagingSession, IListener<MessageRecord>
    {
        private readonly IList<MessageRecord> _all = new List<MessageRecord>();
        private readonly ChannelGraph _graph;

        private readonly ConcurrentCache<string, MessageLog> _histories =
            new ConcurrentCache<string, MessageLog>(id => new MessageLog {Id = id});

        public MessagingSession(ChannelGraph graph)
        {
            _graph = graph;
        }

        public void Receive(MessageRecord message)
        {
            Record(message);
        }

        public void ClearAll()
        {
            _histories.ClearAll();
            _all.Clear();
        }

        public void Record(MessageRecord record)
        {
            if (record == null) return;

            if (_all.Contains(record)) return;
            _all.Add(record);

            // Important, don't override what's already there
            if (record.Node.IsEmpty())
            {
                record.Node = _graph.NodeId;
            }

            // Letting the remote AppDomain's know about it.
            GlobalMessageTracking.SendMessage(record);

            var history = _histories[record.Id];
            history.Record(record);

            if (record.ParentId.IsNotEmpty() && record.ParentId != Guid.Empty.ToString())
            {
                var parent = _histories[record.ParentId];
                parent.AddChild(history); // this is idempotent, so we're all good
            }
        }

        public IEnumerable<MessageLog> TopLevelMessages()
        {
            return _histories.Where(x => x.Parent == null).OrderBy(x => x.Timestamp);
        }

        public IEnumerable<MessageLog> AllMessages()
        {
            return _histories.OrderBy(x => x.Timestamp);
        }

        public IEnumerable<MessageRecord> All()
        {
            return AllMessages().SelectMany(x => x.Records());
        }
    }
}