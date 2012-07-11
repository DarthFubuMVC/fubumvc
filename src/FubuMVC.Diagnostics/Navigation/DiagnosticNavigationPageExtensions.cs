using FubuMVC.Core.UI.Navigation;
using FubuMVC.Core.View;
using FubuMVC.TwitterBootstrap.Menus;
using HtmlTags;

namespace FubuMVC.Diagnostics.Navigation
{
    public static class DiagnosticNavigationPageExtensions
    {
        public static HtmlTag DiagnosticsMenu(this IFubuPage page)
        {
            var service = page.Get<INavigationService>();
            var tokens = service.MenuFor(DiagnosticKeys.Main);

            return new MenuTag(tokens);
        }
    }
}