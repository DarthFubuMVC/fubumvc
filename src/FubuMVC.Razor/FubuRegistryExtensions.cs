using System;
using FubuCore;
using FubuMVC.Core;

namespace FubuMVC.Razor
{
    public static class FubuRegistryExtensions
    {
        public static void UseRazor(this FubuRegistry fubuRegistry)
        {
            fubuRegistry.UseRazor(s => { });
        }

        public static void UseRazor(this FubuRegistry fubuRegistry, Action<RazorEngine> configure)
        {
            var Razor = new RazorEngine();
            configure(Razor);
            Razor
                .As<IFubuRegistryExtension>()
                .Configure(fubuRegistry);
        }
    }
}