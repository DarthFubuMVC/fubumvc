using FubuMVC.Core;

namespace FubuMVC.Nowin
{
    public class NowinExtensions : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services.AddService<IActivator, NowinHostingActivator>();
            registry.Services.AddService<IDeactivator, KatanaHostingDeactivator>();
        }
    }
}
