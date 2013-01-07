using FubuCore.Descriptions;
using FubuMVC.Core.Configuration;

namespace FubuMVC.Core.Registration.Diagnostics
{
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