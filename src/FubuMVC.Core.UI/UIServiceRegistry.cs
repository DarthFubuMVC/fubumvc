using Bottles;
using FubuMVC.Core.Registration;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Security;

namespace FubuMVC.Core.UI
{
    public class UIServiceRegistry : ServiceRegistry
    {
        public UIServiceRegistry()
        {
            SetServiceIfNone<IFieldAccessService, FieldAccessService>();
            SetServiceIfNone<IFieldAccessRightsExecutor, FieldAccessRightsExecutor>();

            SetServiceIfNone<IElementNamingConvention, DefaultElementNamingConvention>();

            AddService<IActivator, HtmlConventionsActivator>();

            AddService<IActivator>(typeof(DisplayConversionRegistryActivator));

            SetServiceIfNone<IPartialInvoker, PartialInvoker>();
        }
    }
}