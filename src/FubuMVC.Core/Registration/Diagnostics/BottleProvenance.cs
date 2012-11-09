using Bottles;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration.Diagnostics
{
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
}