using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Diagnostics;
using System.Linq;

namespace FubuMVC.Core.Registration.Nodes
{
    public class TracedNode : ITracedModel
    {
        private readonly Queue<NodeEvent> _events = new Queue<NodeEvent>();

        public TracedNode()
        {
            Trace(new Created());
        }

        public void Trace(NodeEvent @event)
        {
            _events.Enqueue(@event);
            @event.Subject = this;
        }

        public void Trace(string text)
        {
            _events.Enqueue(new Traced(text));
        }

        IEnumerable<NodeEvent> ITracedModel.StagedEvents
        {
            get { return _events; }
        }

        void ITracedModel.RecordEvents(Action<NodeEvent> callback)
        {
            while (_events.Any())
            {
                var e = _events.Dequeue();
                callback(e);
            }
        }
    }
}