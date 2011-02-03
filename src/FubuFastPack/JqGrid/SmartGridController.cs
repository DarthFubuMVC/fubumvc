using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;
using FubuFastPack.Querying;
using FubuLocalization;
using FubuMVC.Core;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    public class GridRequest
    {
        public int page { get; set; }
        public int rows { get; set; }
        public string sidx { get; set; }
        public string sord { get; set; }
// ReSharper disable InconsistentNaming
        public string _search { get; set; }
// ReSharper restore InconsistentNaming
        public string nd { get; set; }

        public Criteria[] gridOptions { get; set; }
        public Criteria[] criterion { get; set; }
        public string gridName { get; set; }

        // TODO -- put a UT around this.

        public GridDataRequest ToDataRequest()
        {
            bool sortAscending = !"desc".Equals(sord, StringComparison.OrdinalIgnoreCase);
            return new GridDataRequest(page, rows, sidx, sortAscending){
                Criterion = criterion,
                GridOptions = gridOptions
            };
        }
    }

    public class SmartGridRegistry : FubuPackageRegistry
    {
        public SmartGridRegistry()
        {
            this.Route("griddata/{gridname}").Calls<SmartGridController>(x => x.Data(null));
        }
    }

    public class SmartGridController
    {
        private readonly IServiceLocator _services;
        private readonly IQueryService _queryService;
        private readonly IUrlRegistry _urls;

        public SmartGridController(IServiceLocator services, IQueryService queryService, IUrlRegistry urls)
        {
            _services = services;
            _queryService = queryService;
            _urls = urls;
        }

        // Maybe turn this into other requests
        public GridResults Data(GridRequest input)
        {
            var grid = _services.GetInstance<IGrid>(input.gridName);
            return grid.Invoke(_services, input.ToDataRequest());
        }

        public JqGridModel ModelFor<T>() where T : IGrid
        {
            var grid = _services.GetInstance<T>();
            return ModelFor(grid);
        }

        //public JqGridModel ModelFor(string gridName)
        //{
            
        //}

        public JqGridModel ModelFor(IGrid grid)
        {
            string gridName = grid.GetType().NameForGrid();
            return new JqGridModel(){
                baselineCriterion = Enumerable.ToArray<Criteria>(grid.BaselineCriterion),
                colModel = Enumerable.Select<IGridColumn, IDictionary<string, object>>(grid.Definition.Columns, x => x.ToDictionary()).ToArray(),
                filters = Enumerable.ToArray<FilterDTO>(grid.Definition.AllPossibleFilters(_queryService)),
                gridName = gridName,
                url = _urls.UrlFor(new GridRequest{gridName = gridName})
            };
        }
    }

    public static class SmartGridViewExtensions
    {
        public static string NameForGrid(this Type gridType)
        {
            return gridType.Name.Replace("Grid", string.Empty);
        }

        public static string ContainerNameForGrid(this Type gridType)
        {
            var gridName = gridType.NameForGrid();
            return gridName.ContainerNameForGrid();
        }

        public static string Name(this IGrid grid)
        {
            return grid.GetType().NameForGrid();
        }

        public static string ContainerNameForGrid(this string gridName)
        {
            return "gridContainer_" + gridName;
        }

        public static HtmlTag SmartGridFor(this IFubuPage page, IGrid grid, int? initialRows)
        {
            // TODO -- later
            //page.Script("shared/dovetail.smartGrid.js");

            // TODO -- add security!!!!!!!!!!!!

            return new HtmlTag("div", top =>
            {
                string gridName = grid.Name();
                top.Add("div")
                    .AddClass("grid-container")
                    .MetaData("definition", grid)
                    .MetaData("initialRows", initialRows.GetValueOrDefault(10))
                    .Add("table").Id(gridName).AddClass("smartgrid");

                top.Add("div").Id(gridName + "_pager").AddClass("pager-bottom").AddClass("clean");
            });
        }

        private static HtmlTag SmartGridFor<T>(this IFubuPage page, int? initialRows) where T : IGrid
        {
            var tag = page.SmartGridFor(page.Get<T>(), initialRows);
            typeof(T).ForAttribute<AllowRoleAttribute>(att => tag.RequiresAccessTo(att.Roles));

            return tag;
        }
    }
}