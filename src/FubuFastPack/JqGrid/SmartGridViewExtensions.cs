using System;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuFastPack.JqGrid
{
    // TODO -- add tests for this mess
    public static class SmartGridViewExtensions
    {
        public static void ApplySmartGridConventions(this FubuRegistry registry, Action<AppliesToExpression> configure)
        {
            var pool = new TypePool(null);
            pool.IgnoreCallingAssembly();
            pool.ShouldScanAssemblies = true;

            var expression = new AppliesToExpression(pool);
            configure(expression);

            registry.ApplyConvention(new SmartGridConvention(pool));
        }

        public static HtmlTag FiltersFor<T>(this IFubuPage page) where T : ISmartGrid
        {
            var harness = page.Get<SmartGridHarness<T>>();
            var model = harness.BuildGridModel(null);

            return FiltersFor(page, model);
        }

        public static HtmlTag FiltersFor(this IFubuPage page, GridViewModel model)
        {
            page.Script("grid");
            return page.Get<FilterTagWriter>().FilterTemplatesFor(model);
        }


        public static string NameForGrid(this Type gridType)
        {
            return gridType.Name.Replace("Grid", string.Empty);
        }

        public static string ContainerNameForGrid(this Type gridType)
        {
            var gridName = gridType.NameForGrid();
            return gridName.ContainerNameForGrid();
        }

        public static string Name(this ISmartGrid grid)
        {
            return grid.GetType().NameForGrid();
        }

        public static string ContainerNameForGrid(this string gridName)
        {
            return "gridContainer_" + gridName;
        }

        public static string IdForLabel(this Type gridType)
        {
            return gridType.NameForGrid() + "-label";
        }

        public static HtmlTag SmartGridFor<T>(this IFubuPage page, int? initialRows) where T : ISmartGrid
        {
            return page.SmartGridFor<T>(initialRows, h => { });
        }

        public static HtmlTag DisabledSmartGridFor<TGrid>(this IFubuPage page, int? initialRows) where TGrid : ISmartGrid
        {
            var tag = page.SmartGridFor<TGrid>(initialRows, h => { });
            var grid = tag.FirstChild();
            if (grid != null)
            {
                grid.MetaData("disabled", true);
            }
            return tag;
        }

        public static HtmlTag SmartGridFor<TGrid>(this IFubuPage page, int? initialRows, params object[] arguments)
            where TGrid : ISmartGrid
        {
            return page.SmartGridFor<TGrid>(initialRows, h => h.RegisterArguments(arguments));
        }

        // TODO -- End to End stuff on this one
        private static HtmlTag SmartGridFor<T>(this IFubuPage page, int? initialRows,
                                               Action<SmartGridHarness<T>> modification) where T : ISmartGrid
        {
            var endpoint = page.Get<IEndpointService>().EndpointFor<SmartGridHarness<T>>(x => x.Data(null));
            if (!endpoint.IsAuthorized)
            {
                return HtmlTag.Empty();
            }

            var harness = page.Get<SmartGridHarness<T>>();
            modification(harness);

            var model = harness.BuildJqModel();


            return SmartGridFor(page, model, initialRows);
        }

        public static HtmlTag SmartGridFor(this IFubuPage page, JqGridModel model, int? initialRows)
        {
            page.Script("grid");
            return new HtmlTag("div", top =>
            {
                string gridName = model.gridName;
                top.Add("div")
                    .Id("grid_" + gridName)
                    .AddClass("grid-container")
                    .MetaData("definition", model)
                    .MetaData("initialRows", initialRows.GetValueOrDefault(10))
                    .MetaData("disabled", false)
                    .Add("table").Id(model.containerName).AddClass("smartgrid");

                top.Add("div").Id(gridName + "_pager").AddClass("pager-bottom").AddClass("grid-pager").AddClass("clean");
            });
        }
    }
}