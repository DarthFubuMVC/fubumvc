using System;
using System.Collections.Generic;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public class ConfigSource
    {
        private readonly IConfigurationAction _action;
        private readonly Lazy<Description> _description;
        private readonly IList<NodeEvent> _events = new List<NodeEvent>();

        public ConfigSource(IConfigurationAction action)
        {
            _action = action;
            Id = Guid.NewGuid();
            _description = new Lazy<Description>(() => Description.For(action));
        }

        public IConfigurationAction Action
        {
            get { return _action; }
        }

        public Guid Id { get; private set;}

        public Description Description
        {
            get { return _description.Value; }
        }

        public void AddEvent(NodeEvent @event)
        {
            @event.Source = this;
            _events.Add(@event);
        }

        public IList<NodeEvent> Events
        {
            get { return _events; }
        }
    }
}