using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public class ActionLog
    {
        private readonly IConfigurationAction _action;
        private readonly IEnumerable<Provenance> _provenanceChain;
        private readonly Lazy<Description> _description;
        private readonly IList<NodeEvent> _events = new List<NodeEvent>();

        public ActionLog(IConfigurationAction action, IEnumerable<Provenance> provenanceChain)
        {
            _action = action;
            _provenanceChain = provenanceChain.ToArray();
            Id = Guid.NewGuid();
            _description = new Lazy<Description>(() => Description.For(action));
        }

        public IEnumerable<Provenance> ProvenanceChain
        {
            get { return _provenanceChain; }
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

        protected bool Equals(ActionLog other)
        {
            return Equals(_action, other._action);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ActionLog) obj);
        }

        public override int GetHashCode()
        {
            return (_action != null ? _action.GetHashCode() : 0);
        }
    }
}