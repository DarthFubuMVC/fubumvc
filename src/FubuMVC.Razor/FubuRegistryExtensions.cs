using System;
using FubuCore;
using FubuMVC.Core;

namespace FubuMVC.Razor
{
    public static class FubuRegistryExtensions
    {
        [Obsolete("This call is completely unnecessary if the FubuMVC.Razor assembly is in the application path")]
        public static void UseRazor(this FubuRegistry fubuRegistry)
        {
            fubuRegistry.UseRazor(s => { });
        }

        [Obsolete("Use FubuRegistry.Import<RazorEngineRegistry>(Action<RazorEngineRegistry>) instead")]
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