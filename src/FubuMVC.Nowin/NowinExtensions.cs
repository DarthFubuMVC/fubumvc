using Bottles;
using FubuMVC.Core;

namespace FubuMVC.Nowin
{
    public class NowinExtensions : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services(x => {
                x.AddService<IActivator, NowinHostingActivator>();
                x.AddService<IDeactivator, KatanaHostingDeactivator>();
            });
        }
    }
}
