using FubuCore;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.SlickGrid
{
    // Depending on manual and E2E tests for this bad boy
    public static class GridFubuPageExtensions
    {
        public static HtmlTag RenderGrid<T>(this IFubuPage page, string id) where T : IGridDefinition, new()
        {
            var grid = new T();

            var div = new HtmlTag("div").Id(id).AddClass("slick-grid");
            div.Data("columns", grid.ToColumnJson());

            page.Asset("diagnostics_styles", "diagnostics");
            page.Asset("diagnostics/SlickGridActivator.js");


            var url = grid.SelectDataSourceUrl(page.Urls);
            if (url.IsNotEmpty())
            {
                div.Data("url", url);
            }

            return div;
        }
    }
}