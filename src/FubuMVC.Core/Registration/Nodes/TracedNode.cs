using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Diagnostics;
using System.Linq;

namespace FubuMVC.Core.Registration.Nodes
{
    public abstract class TracedNode : ITracedModel
    {
        private readonly Queue<NodeEvent> _events = new Queue<NodeEvent>();

        protected TracedNode()
        {
            Trace(new Created(this));
        }

        public void Trace(NodeEvent @event)
        {
            _events.Enqueue(@event);
        }

        public void Trace(string text)
        {
            _events.Enqueue(new Traced(text, this));
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