using FubuMVC.Core;

namespace FubuMVC.Katana
{
    public class KatanaExtensions : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services.AddService<IActivator, KatanaHostingActivator>();
            registry.Services.AddService<IDeactivator, KatanaHostingDeactivator>();
        }
    }
}