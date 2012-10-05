namespace FubuMVC.Core.Assets
{
    public class AssetBottleRegistration : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services<AssetServicesRegistry>();
            registry.Policies.Add<AssetContentEndpoint>();
        }
    }
}