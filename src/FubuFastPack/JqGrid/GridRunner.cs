using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
using FubuMVC.Core.Urls;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    public interface IGridRunner<TEntity, TService>
    {
        TService Service { get; }

        GridResults RunGrid<T>(GridDefinition<T> grid, IGridDataSource<T> source, PagingOptions paging);
    }

    public class GridRunner<TEntity, TService> : IGridRunner<TEntity, TService> where TEntity : DomainEntity
    {
        private readonly IDisplayFormatter _formatter;
        private readonly IUrlRegistry _urls;
        private readonly IServiceLocator _services;
        private readonly IObjectConverter _converter;
        private readonly TService _service;

        public GridRunner(IDisplayFormatter formatter, IUrlRegistry urls, IServiceLocator services, IObjectConverter converter, TService service)
        {
            _formatter = formatter;
            _urls = urls;
            _services = services;
            _converter = converter;
            _service = service;
        }

        public TService Service
        {
            get { return _service; }
        }

        public GridResults RunGrid<T>(GridDefinition<T> grid, IGridDataSource<T> source, PagingOptions paging)
        {
            var data = source.Fetch(paging);
            var actions = grid.Columns.Select(col => col.CreateFiller(data, _formatter, _urls));

            // Need to apply criteria here

            List<EntityDTO> list = createEntityDtos(data, actions);

            // TODO -- needs some UT's
            return ApplyPaging(source, paging, list);
        }

        private List<EntityDTO> createEntityDtos(IGridData data, IEnumerable<Action<EntityDTO>> actions)
        {
            var list = new List<EntityDTO>();

            while (data.MoveNext())
            {
                var dto = new EntityDTO();
                actions.Each(a => a(dto));

                list.Add(dto);
            }
            return list;
        }

        public GridResults ApplyPaging<T>(IGridDataSource<T> source, PagingOptions paging, List<EntityDTO> list)
        {
            int recordCount = source.TotalCount();
            var pageCount = (int)Math.Ceiling(recordCount / (double)paging.ResultsPerPage);

            if (pageCount < paging.Page)
            {
                paging.Page = pageCount;
            }

            return new GridResults()
                   {
                       page = paging.Page,
                       records = recordCount,
                       total = pageCount,
                       items = list
                   };
        }

        public IEnumerable<FilterRequest<T>> CreateFilterRequests<T>(GridDefinition<T> grid, IEnumerable<Criteria> criterion)
        {
            return criterion.Select(x => new FilterRequest<T>(x, _converter, grid.PropertyFor(x.property)));
        }

    }
}