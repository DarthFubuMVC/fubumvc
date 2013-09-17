using Bottles;
using FubuMVC.Core;

namespace FubuMVC.Katana
{
    public class KatanaExtensions : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services(x => {
                x.AddService<IActivator, KatanaHostingActivator>();
                x.AddService<IDeactivator, KatanaHostingDeactivator>();
            });
        }
    }
}