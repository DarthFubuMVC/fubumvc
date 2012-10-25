using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public abstract class Provenance : DescribesItself
    {
        public abstract void Describe(Description description);
    }

    public class BottleProvenance : Provenance
    {
        private readonly string _bottleName;

        public BottleProvenance(string bottleName)
        {
            _bottleName = bottleName;
        }

        public override void Describe(Description description)
        {
            description.Title = "Bottle: " + _bottleName;
        }
    }

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
    }

    public class FubuRegistryExtensionProvenance : Provenance
    {
        private readonly IFubuRegistryExtension _extension;

        public FubuRegistryExtensionProvenance(IFubuRegistryExtension extension)
        {
            _extension = extension;
        }

        public IFubuRegistryExtension Extension
        {
            get { return _extension; }
        }

        protected bool Equals(FubuRegistryExtensionProvenance other)
        {
            return Equals(_extension, other._extension);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FubuRegistryExtensionProvenance) obj);
        }

        public override int GetHashCode()
        {
            return (_extension != null ? _extension.GetHashCode() : 0);
        }

        public override void Describe(Description description)
        {
            description.Title = "IFubuRegistryExtension: " + _extension.GetType().Name;
            description.Children["Extension"] = Description.For(_extension);
        }
    }

    public class ConfigurationPackProvenance : Provenance
    {
        private readonly ConfigurationPack _pack;

        public ConfigurationPackProvenance(ConfigurationPack pack)
        {
            _pack = pack;
        }

        public ConfigurationPack Pack
        {
            get { return _pack; }
        }

        protected bool Equals(ConfigurationPackProvenance other)
        {
            return Equals(_pack, other._pack);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConfigurationPackProvenance) obj);
        }

        public override int GetHashCode()
        {
            return (_pack != null ? _pack.GetHashCode() : 0);
        }

        public override void Describe(Description description)
        {
            description.Title = "ConfigurationPack:  " + _pack.GetType().Name;
            description.Children["Pack"] = Description.For(_pack);
        }
    }


}