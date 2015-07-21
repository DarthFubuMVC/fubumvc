using System;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Execution
{
    public class ScheduledJobRecordModifier : ILogModifier
    {
        private readonly ChannelGraph _graph;

        public ScheduledJobRecordModifier(ChannelGraph graph)
        {
            _graph = graph;
        }

        public bool Matches(Type logType)
        {
            return logType.CanBeCastTo<ScheduledJobRecord>();
        }

        public void Modify(object log)
        {
            log.As<ScheduledJobRecord>().NodeId = _graph.NodeId;
        }
    }
}