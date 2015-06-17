using System;
using FubuCore;
using FubuCore.Logging;
using FubuTransportation.Configuration;

namespace FubuTransportation.Monitoring
{
    public class PersistentTaskMessageModifier : ILogModifier
    {
        private readonly ChannelGraph _graph;

        public PersistentTaskMessageModifier(ChannelGraph graph)
        {
            _graph = graph;
        }

        public bool Matches(Type logType)
        {
            return logType.CanBeCastTo<PersistentTaskMessage>();
        }

        public void Modify(object log)
        {
            var message = log.As<PersistentTaskMessage>();

            message.Machine = Environment.MachineName;
            message.NodeId = _graph.NodeId;
        }
    }
}