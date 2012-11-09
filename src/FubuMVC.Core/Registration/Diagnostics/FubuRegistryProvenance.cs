using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public class FubuRegistryProvenance : Provenance
    {
        private readonly FubuRegistry _registry;

        public FubuRegistryProvenance(FubuRegistry registry)
        {
            _registry = registry;
        }

        public FubuRegistry Registry
        {
            get { return _registry; }
        }

        protected bool Equals(FubuRegistryProvenance other)
        {
            return Equals(_registry, other._registry);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FubuRegistryProvenance) obj);
        }

        public override int GetHashCode()
        {
            return (_registry != null ? _registry.GetHashCode() : 0);
        }

        public override void Describe(Description description)
        {
            description.Title = "FubuRegistry:  " + _registry.GetType().Name;
            description.Children["Registry"] = Description.For(_registry);
        }

        public override string ToString()
        {
            return _registry.GetType().Name;
        }
    }
}