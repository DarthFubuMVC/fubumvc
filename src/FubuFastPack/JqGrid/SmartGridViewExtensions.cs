using System;
using FubuCore.Reflection;
using FubuFastPack.Querying;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuFastPack.JqGrid
{
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

        // TODO -- move to interface?
        public static HtmlTag FiltersFor<T>(this IFubuPage page) where T : ISmartGrid
        {
            page.Script("grid");
            return page.Get<FilterTagWriter>().FilterTemplatesFor<T>();
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

        // TODO -- End to End stuff on this one
        public static HtmlTag SmartGridFor<T>(this IFubuPage page, int? initialRows) where T : ISmartGrid
        {
            var endpoint = page.Get<IEndpointService>().EndpointFor<SmartGridController<T>>(x => x.Data(null));
            if (!endpoint.IsAuthorized)
            {
                return HtmlTag.Empty();
            }

            ISmartGrid grid = page.Get<T>();
            
            page.Script("grid");

            var model = page.Get<SmartGridController<T>>().BuildModel(grid);
            return new HtmlTag("div", top =>
            {
                string gridName = grid.Name();
                top.Add("div")
                    .Id(grid.GetType().ContainerNameForGrid())
                    .AddClass("grid-container")
                    .MetaData("definition", model)
                    .MetaData("initialRows", initialRows.GetValueOrDefault(10))
                    .Add("table").Id(gridName).AddClass("smartgrid");

                top.Add("div").Id(gridName + "_pager").AddClass("pager-bottom").AddClass("grid-pager").AddClass("clean");
            });
        }
    }
}