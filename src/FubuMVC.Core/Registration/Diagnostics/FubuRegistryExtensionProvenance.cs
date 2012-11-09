using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration.Diagnostics
{
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
}