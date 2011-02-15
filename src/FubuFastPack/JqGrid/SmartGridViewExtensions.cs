using System;
using FubuFastPack.Domain;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.DSL;
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
            throw new NotImplementedException();

            //page.Script("grid");
            //return page.Get<FilterTagWriter>().FilterTemplatesFor<T>();
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

        public static HtmlTag SmartGridFor<T>(this IFubuPage page, int? initialRows) where T : ISmartGrid
        {
            return page.SmartGridFor<T>(initialRows, h => { });
        }

        public static HtmlTag SmartGridFor<TGrid, TEntity>(this IFubuPage page, int? initialRows, TEntity subject)
            where TGrid : ISmartGrid where TEntity : DomainEntity
        {
            throw new NotImplementedException();
            //return page.SmartGridFor<TGrid>(initialRows, h => h.RegisterArgument(typeof(TEntity), subject));
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

            page.Script("grid");

            var harness = page.Get<SmartGridHarness<T>>();
            modification(harness);

            var model = harness.BuildJqModel();
            return new HtmlTag("div", top =>
            {
                string gridName = typeof (T).NameForGrid();
                top.Add("div")
                    .Id(typeof (T).ContainerNameForGrid())
                    .AddClass("grid-container")
                    .MetaData("definition", model)
                    .MetaData("initialRows", initialRows.GetValueOrDefault(10))
                    .Add("table").Id(gridName).AddClass("smartgrid");

                top.Add("div").Id(gridName + "_pager").AddClass("pager-bottom").AddClass("grid-pager").AddClass("clean");
            });
        }
    }
}