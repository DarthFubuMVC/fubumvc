using FubuLocalization;
using FubuMVC.Core.UI.Navigation;
using FubuMVC.Core.View;
using FubuMVC.Diagnostics.Features.Dashboard;
using FubuMVC.Diagnostics.Features.Html;
using FubuMVC.Diagnostics.Features.Packaging;
using FubuMVC.Diagnostics.Features.Requests;
using FubuMVC.Diagnostics.Features.Routes;
using HtmlTags;
using System.Collections.Generic;

namespace FubuMVC.Diagnostics
{
    public class DiagnosticKeys : StringToken
    {
        public static readonly DiagnosticKeys Main = new DiagnosticKeys("Main");
        public static readonly DiagnosticKeys Dashboard = new DiagnosticKeys("Dashboard");
        public static readonly DiagnosticKeys HtmlConventions = new DiagnosticKeys("Html Conventions");
        public static readonly DiagnosticKeys ApplicationStartup = new DiagnosticKeys("Application Startup");
        public static readonly DiagnosticKeys Requests = new DiagnosticKeys("Requests");
        public static readonly DiagnosticKeys Routes = new DiagnosticKeys("Routes");

        public DiagnosticKeys(string defaultValue) : base(null, defaultValue, namespaceByType: true)
        {
        }

        public bool Equals(DiagnosticKeys other)
        {
            return other.Key.Equals(Key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as DiagnosticKeys);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }

    public class DiagnosticsMenu : NavigationRegistry
    {
        public DiagnosticsMenu()
        {
            // TODO -- Don't like forcing you to use the StringToken
            ForMenu(DiagnosticKeys.Main);
            Add += MenuNode.ForInput<DashboardRequestModel>(DiagnosticKeys.Dashboard);
            Add += MenuNode.ForInput<HtmlConventionsRequestModel>(DiagnosticKeys.HtmlConventions);
            Add += MenuNode.ForInput<PackageDiagnosticsRequestModel>(DiagnosticKeys.ApplicationStartup);
            Add += MenuNode.ForInput<RequestExplorerRequestModel>(DiagnosticKeys.Requests);
            Add += MenuNode.ForInput<DefaultRouteRequestModel>(DiagnosticKeys.Routes);
        }
    }

    public static class DiagnosticNavigationPageExtensions
    {
        public static HtmlTag DiagnosticsMenu(this IFubuPage page)
        {
            var service = page.Get<INavigationService>();
            var tokens = service.MenuFor(DiagnosticKeys.Main);

            var tag = new HtmlTag("ul").AddClass("nav");
            tokens.Each(token => tag.Append(new DiagnosticMenuItemTag(token)));


            return tag;
        }
    }

    // TODO -- add tests
    public class DiagnosticMenuItemTag : HtmlTag
    {
        public DiagnosticMenuItemTag(MenuItemToken item) : base("li")
        {
            var link = Add("a").Attr("href", item.Url).Text(item.Text);
            if (item.MenuItemState == MenuItemState.Active)
            {
                link.AddClass("active");
            }
        }
    }
}