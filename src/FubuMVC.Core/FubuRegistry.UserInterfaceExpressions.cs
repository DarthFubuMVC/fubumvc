using System;
using System.Linq;
using FubuCore.Formatting;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.UI;
using FubuCore;

namespace FubuMVC.Core
{
    public partial class FubuRegistry : IFubuRegistry
    {
        [Obsolete("Change to using FubuRegistry.Import<T>().  This method will be removed by FubuMVC 1.0")]
        public void HtmlConvention<T>() where T : HtmlConventionRegistry, new()
        {
            Import<T>();
        }

        [Obsolete("Change to using FubuRegistry.Import(conventions).  This method will be removed by FubuMVC 1.0")]
        public void HtmlConvention(HtmlConventionRegistry conventions)
        {
            conventions.As<IFubuRegistryExtension>().Configure(this);
        }

        [Obsolete("Change to using FubuRegistry.Import<HtmlConventionRegistry>(Action<HtmlConventionRegistry>).  This method will be removed by FubuMVC 1.0")]
        public void HtmlConvention(Action<HtmlConventionRegistry> configure)
        {
            Import(configure);
        }

        [Obsolete("Change to using FubuRegistry.Policies.StringConversions<T>()")]
        public void StringConversions<T>() where T : DisplayConversionRegistry, new()
        {
            Policies.StringConversions<T>();
        }

        [Obsolete("Change to using FubuRegistry.Policies.StringConversions(Action<DisplayConversionRegistry>)")]
        public void StringConversions(Action<DisplayConversionRegistry> configure)
        {
            Policies.StringConversions(configure);
        }
    }
}