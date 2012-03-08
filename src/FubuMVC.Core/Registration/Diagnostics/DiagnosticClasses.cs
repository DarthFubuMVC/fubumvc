using System;
using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public class ConfigLog
    {
        public void StartSource(IConfigurationAction action)
        {
            throw new NotImplementedException();
        }

        public void RecordEvents(BehaviorGraph graph)
        {
            
        }

        public void RecordEvents(BehaviorNode node)
        {
            throw new NotImplementedException();
        }        

        public void RecordNewEndpoint(BehaviorChain chain)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<NodeEvent> EventsFor(BehaviorNode node)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<NodeEvent> EventsFor(BehaviorChain chain)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ConfigSource> AllConfigSources()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<NodeEvent> EventsFor(ConfigSource source)
        {
            throw new NotImplementedException();
        }

       
    }

    public class ConfigSource
    {
        private readonly IConfigurationAction _action;
        private Lazy<Description> _description;

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

        public void AddEvent(BehaviorNode node, NodeEvent @event)
        {
            @event.Source = this;
        }


    }

    public abstract class NodeEvent
    {
        private readonly object _subject;

        public NodeEvent(object subject)
        {
            _subject = subject;
        }

        public object Subject
        {
            get { return _subject; }
        }

        public ConfigSource Source { get; set; }
    }

    public class Created : NodeEvent
    {
        public Created(object subject) : base(subject)
        {
        }
    }

    public class Traced : NodeEvent
    {
        private readonly string _text;

        public Traced(string text, object subject) : base(subject)
        {
            _text = text;
        }

        public string Text
        {
            get { return _text; }
        }
    }
}