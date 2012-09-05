using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Diagnostics;

namespace FubuMVC.Core.Registration.Nodes
{
    public interface ITracedModel
    {
        void Trace(NodeEvent @event);
        void Trace(string text, params object[] parameters);
        IEnumerable<NodeEvent> StagedEvents { get; }
        void RecordEvents(Action<NodeEvent> callback);
        IEnumerable<NodeEvent> AllEvents();
    }
}