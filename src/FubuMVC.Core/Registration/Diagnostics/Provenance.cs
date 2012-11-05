using System;
using Bottles;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public abstract class Provenance : DescribesItself
    {
        public readonly Guid Id = Guid.NewGuid();

        public abstract void Describe(Description description);


        private readonly Lazy<Description> _description;

        protected Provenance()
        {
            _description = new Lazy<Description>(() => Description.For(this));
        }

        public Description Description
        {
            get { return _description.Value; }
        }
    }

    public class BottleProvenance : Provenance
    {
        private readonly IPackageInfo _bottle;

        public BottleProvenance(IPackageInfo bottle)
        {
            _bottle = bottle;
        }

        public override void Describe(Description description)
        {
            description.Title = "Bottle: " + _bottle.Name;
        }

        protected bool Equals(BottleProvenance other)
        {
            return Equals(_bottle, other._bottle);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BottleProvenance) obj);
        }

        public override int GetHashCode()
        {
            return (_bottle != null ? _bottle.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Bottle: {0}", _bottle.Name);
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

        public override string ToString()
        {
            return _registry.GetType().Name;
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

        public override string ToString()
        {
            return string.Format("Extension: {0}", _extension.GetType().Name);
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

        public override string ToString()
        {
            return _pack.GetType().Name;
        }
    }


}