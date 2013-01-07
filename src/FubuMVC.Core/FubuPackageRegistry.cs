namespace FubuMVC.Core
{
    /// <summary>
    /// The base FubuRegistry for importing endpoints from an external Bottle.  This 
    /// is primarily used for "module" Bottles that add vertical slices of functionality
    /// to a base application
    /// </summary>
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