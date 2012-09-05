using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Diagnostics;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Registration.Nodes
{
    public class TracedNode : ITracedModel
    {
        private readonly Queue<NodeEvent> _stagedEvents = new Queue<NodeEvent>();
        private readonly IList<NodeEvent> _events = new List<NodeEvent>();

        public TracedNode()
        {
            Trace(new Created());
        }

        public void Trace(NodeEvent @event)
        {
            _stagedEvents.Enqueue(@event);
            @event.Subject = this;
        }

        public void Trace(string text, params object[] parameters)
        {
            var message = parameters.Any() ? text.ToFormat(parameters) : text;
            _stagedEvents.Enqueue(new Traced(message));
        }

        IEnumerable<NodeEvent> ITracedModel.StagedEvents
        {
            get { return _stagedEvents; }
        }


        void ITracedModel.RecordEvents(Action<NodeEvent> callback)
        {
            while (_stagedEvents.Any())
            {
                var e = _stagedEvents.Dequeue();
                e.Subject = this;

                callback(e);

                _events.Add(e);
            }
        }

        IEnumerable<NodeEvent> ITracedModel.AllEvents()
        {
            return _events;
        }
    }
}