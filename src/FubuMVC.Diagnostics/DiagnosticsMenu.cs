using FubuLocalization;
using FubuMVC.Core.Assets;
using FubuMVC.Core.UI.Navigation;
using FubuMVC.Core.View;
using FubuMVC.Diagnostics.Features.Dashboard;
using FubuMVC.Diagnostics.Features.Html;
using FubuMVC.Diagnostics.Features.Packaging;
using FubuMVC.Diagnostics.Features.Requests;
using FubuMVC.Diagnostics.Features.Routes;
using HtmlTags;
using System.Collections.Generic;
using FubuCore;
using System.Linq;
using FubuMVC.Core.UI;

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
            return ("DiagnosticKeys:" + Key).GetHashCode();
        }
    }

    public class DiagnosticsMenu : NavigationRegistry
    {
        public DiagnosticsMenu()
        {
            ForMenu(DiagnosticKeys.Main);
            Add += MenuNode.ForInput<DashboardRequestModel>(DiagnosticKeys.Dashboard);
            Add += MenuNode.ForInput<HtmlConventionsRequestModel>(DiagnosticKeys.HtmlConventions);
            Add += MenuNode.ForInput<PackageDiagnosticsRequestModel>(DiagnosticKeys.ApplicationStartup);
            
            // TODO -- add it back in!
            //Add += MenuNode.ForInput<RequestExplorerRequestModel>(DiagnosticKeys.Requests);
            Add += MenuNode.ForInput<DefaultRouteRequestModel>(DiagnosticKeys.Routes);
        }
    }

    public static class DiagnosticNavigationPageExtensions
    {
        public static HtmlTag DiagnosticsMenu(this IFubuPage page)
        {
            var service = page.Get<INavigationService>();
            var tokens = service.MenuFor(DiagnosticKeys.Main);

            return new BootstrapMenuTag(tokens);
        }
    }

    public class BootstrapMenuTag : HtmlTag
    {
        public BootstrapMenuTag(IEnumerable<MenuItemToken> tokens) : base("ul")
        {
            AddClass("nav");
            tokens.Each(token => Append(new BootstrapMenuItemTag(token)));
        }
    }

    // TODO -- add tests
    public class BootstrapMenuItemTag : HtmlTag
    {
        public BootstrapMenuItemTag(MenuItemToken item) : base("li")
        {
            var link = Add("a").Append(new LiteralTag(item.Text));
            if (item.Url.IsNotEmpty())
            {
                link.Attr("href", item.Url);
            }

            if (item.Children.Any())
            {
                link.Attr("href", "#");
                link.AddClass("dropdown-toggle");
                link.Attr("data-toggle", "dropdown");

                link.Add("b").AddClass("caret");

                var ul = Add("ul").AddClass("dropdown-menu");
                item.Children.Each(child =>
                {
                    var childTag = new BootstrapMenuItemTag(child);
                    ul.Append(childTag);
                });
            
            }

            if (item.MenuItemState == MenuItemState.Active)
            {
                AddClass("active");
            }
        }
    }
}