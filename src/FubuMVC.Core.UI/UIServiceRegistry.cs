using Bottles;
using FubuHtml.Elements;
using FubuHtml.Security;
using FubuMVC.Core.Registration;

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