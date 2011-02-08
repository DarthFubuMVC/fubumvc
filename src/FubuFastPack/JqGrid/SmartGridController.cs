using System;
using System.Linq;
using FubuCore.Reflection;
using FubuFastPack.Querying;
using FubuMVC.Core;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    public class GridRequest : JsonMessage
    {
        public int page { get; set; }
        public int rows { get; set; }
        public string sidx { get; set; }
        public string sord { get; set; }
// ReSharper disable InconsistentNaming
        public string _search { get; set; }
// ReSharper restore InconsistentNaming
        public string nd { get; set; }

        public Criteria[] criterion { get; set; }
        public string gridName { get; set; }

        // TODO -- put a UT around this.

        public GridDataRequest ToDataRequest()
        {
            var sortAscending = !"desc".Equals(sord, StringComparison.OrdinalIgnoreCase);
            return new GridDataRequest(page, rows, sidx, sortAscending){
                Criterion = criterion
            };
        }
    }

    public class SmartGridRegistry : FubuPackageRegistry
    {
        // TODO -- very temporary
        public SmartGridRegistry()
        {
            Actions.IncludeMethods(
                call =>
                call.HandlerType == typeof (SmartGridController) && call.Method.HasAttribute<UrlPatternAttribute>());
        }
    }

    public class SmartGridController
    {
        private readonly IServiceLocator _services;
        private readonly IUrlRegistry _urls;

        public SmartGridController(IServiceLocator services, IUrlRegistry urls)
        {
            _services = services;
            _urls = urls;
        }

        // Maybe turn this into other requests
        [UrlPattern("griddata/{gridName}")]
        public GridResults Data(GridRequest input)
        {
            var grid = _services.GetInstance<ISmartGrid>(input.gridName);
            return grid.Invoke(_services, input.ToDataRequest());
        }

        public JqGridModel ModelFor<T>() where T : ISmartGrid
        {
            var grid = _services.GetInstance<T>();
            return ModelFor(grid);
        }


        // TODO -- lots of unit tests here
        public JqGridModel ModelFor(ISmartGrid grid)
        {
            var gridName = grid.GetType().NameForGrid();
            return new JqGridModel{
                colModel = grid.Definition.Columns.SelectMany(x => x.ToDictionary()).ToArray(),
                gridName = gridName,
                url = _urls.UrlFor(new GridRequest{
                    gridName = gridName
                }),
                headers = grid.Definition.Columns.Select(x => x.GetHeader()).ToArray(),
                pagerId = gridName + "_pager",
            };
        }
    }

    public static class SmartGridViewExtensions
    {
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

        public static HtmlTag SmartGridFor(this IFubuPage page, ISmartGrid grid, int? initialRows)
        {
            // TODO -- later
            page.Script("grid");

            // TODO -- add security!!!!!!!!!!!!

            var model = page.Get<SmartGridController>().ModelFor(grid);

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

        public static HtmlTag SmartGridFor<T>(this IFubuPage page, int? initialRows) where T : ISmartGrid
        {
            var tag = page.SmartGridFor(page.Get<T>(), initialRows);
            typeof (T).ForAttribute<AllowRoleAttribute>(att => tag.RequiresAccessTo(att.Roles));

            return tag;
        }
    }
}