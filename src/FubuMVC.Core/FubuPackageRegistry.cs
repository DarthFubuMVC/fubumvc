namespace FubuMVC.Core
{
    public class FubuPackageRegistry : FubuRegistry, IFubuRegistryExtension
    {
        private readonly string _urlPrefix;

        public FubuPackageRegistry() : this(string.Empty)
        {
        }

        public FubuPackageRegistry(string urlPrefix)
        {
            _urlPrefix = urlPrefix;
        }

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            registry.Import(this, _urlPrefix);
        }
    }
}