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

        public static void UseRazor(this FubuRegistry fubuRegistry, Action<RazorEngineRegistry> configure)
        {
            var Razor = new RazorEngineRegistry();
            configure(Razor);
            Razor
                .As<IFubuRegistryExtension>()
                .Configure(fubuRegistry);
        }
    }
}