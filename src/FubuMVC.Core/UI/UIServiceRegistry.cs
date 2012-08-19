using Bottles;
using FubuMVC.Core.Registration;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Tags;

namespace FubuMVC.Core.UI
{
    public class UIServiceRegistry : ServiceRegistry
    {
        public UIServiceRegistry()
        {
            SetServiceIfNone(typeof(ITagGenerator<>), typeof(TagGenerator<>));
            SetServiceIfNone<IElementNamingConvention, DefaultElementNamingConvention>();

            AddService<IActivator, HtmlConventionsActivator>();

            AddService<IActivator>(typeof(DisplayConversionRegistryActivator));

            SetServiceIfNone<IPartialInvoker, PartialInvoker>();
        }
    }
}