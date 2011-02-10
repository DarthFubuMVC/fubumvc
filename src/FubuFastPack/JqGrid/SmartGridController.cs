using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.Urls;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    public class SmartGridController<T> where T : ISmartGrid
    {
        private readonly IServiceLocator _services;
        private readonly IUrlRegistry _urls;
        private readonly T _grid;

        public SmartGridController(IServiceLocator services, IUrlRegistry urls, T grid)
        {
            _services = services;
            _urls = urls;
            _grid = grid;
        }

        public GridResults Data(GridRequest<T> input)
        {
            return _grid.Invoke(_services, input.ToDataRequest());
        }

        public JqGridModel BuildModel()
        {
            var grid = _services.GetInstance<T>();
            return BuildModel(grid);
        }


        // TODO -- lots of unit tests here
        public JqGridModel BuildModel(ISmartGrid grid)
        {
            var gridName = grid.GetType().NameForGrid();
            return new JqGridModel{
                colModel = grid.Definition.Columns.SelectMany(x => x.ToDictionary()).ToArray(),
                gridName = gridName,
                url = _urls.UrlFor(new GridRequest<T>{
                    gridName = gridName
                }),
                headers = grid.Definition.Columns.Select(x => x.GetHeader()).ToArray(),
                pagerId = gridName + "_pager",
            };
        }
    }
}