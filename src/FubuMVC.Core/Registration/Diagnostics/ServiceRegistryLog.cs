using System;
using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuCore;
using FubuCore.Reflection;


namespace FubuMVC.Core.Registration.Diagnostics
{
    public class ServiceRegistryLog : ISourceLog
    {
        private readonly ServiceRegistry _registry;
        private readonly ProvenanceChain _provenanceChain;
        private readonly Lazy<Description> _description;
        private readonly IList<NodeEvent> _events = new List<NodeEvent>();

        public ServiceRegistryLog(ServiceRegistry registry, ProvenanceChain provenanceChain)
        {
            _registry = registry;
            _provenanceChain = registry.GetType() == typeof(ServiceRegistry) ? provenanceChain : provenanceChain.Push(new ServiceRegistryProvenance(registry));
            Id = Guid.NewGuid();
            _description = new Lazy<Description>(() => Description.For(registry));
        }

        public void Apply(ServiceGraph services)
        {
            _registry.As<IServiceRegistration>().Apply(services);
            services.As<ITracedModel>().RecordEvents(AddEvent);
        }

        public ProvenanceChain ProvenanceChain
        {
            get { return _provenanceChain; }
        }

        public ServiceRegistry Registry
        {
            get { return _registry; }
        }

        public void RunAction(ServiceGraph graph)
        {
            _registry.As<IServiceRegistration>().Apply(graph);

            graph.As<ITracedModel>().RecordEvents(AddEvent);
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

        protected bool Equals(ServiceRegistryLog other)
        {
            return Equals(_registry, other._registry);
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
            return (_registry != null ? _registry.GetHashCode() : 0);
        }
    }
}