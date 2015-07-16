using FubuMVC.Core;

namespace FubuMVC.AntiForgery
{
    public class AntiForgeryExtensions : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services<AntiForgeryServiceRegistry>();
            registry.Policies.Global.Add<AntiForgeryPolicy>();
        }
    }
}