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
            var razor = new RazorEngineRegistry();
            configure(razor);
            razor
                .As<IFubuRegistryExtension>()
                .Configure(fubuRegistry);
        }
    }
}