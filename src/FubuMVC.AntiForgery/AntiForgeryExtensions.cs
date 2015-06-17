using FubuMVC.Core;
using FubuMVC.Core.UI;

namespace FubuMVC.AntiForgery
{
    public class AntiForgeryExtensions : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services<AntiForgeryServiceRegistry>();
            registry.Import<HtmlConventionRegistry>(x => x.Forms.Add(new AntiForgeryTagModifier()));
            registry.Policies.Global.Add<AntiForgeryPolicy>();
        }
    }
}