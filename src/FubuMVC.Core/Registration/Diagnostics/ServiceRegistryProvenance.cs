using System;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public class ServiceRegistryProvenance : Provenance
    {
        private readonly ServiceRegistry _registry;

        public ServiceRegistryProvenance(ServiceRegistry registry)
        {
            _registry = registry;
        }

        public override void Describe(Description description)
        {
            // TODO -- pull out of the description from _registry?
            description.Title = "Services: " + _registry.GetType().Name;
        }

        protected bool Equals(ServiceRegistryProvenance other)
        {
            return Equals(_registry, other._registry);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ServiceRegistryProvenance) obj);
        }

        public override int GetHashCode()
        {
            return (_registry != null ? _registry.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Services: {0}", _registry.GetType().Name);
        }
    }
}