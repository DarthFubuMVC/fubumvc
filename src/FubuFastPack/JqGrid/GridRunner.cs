using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
using FubuMVC.Core.Urls;
using FubuFastPack.JqGrid;

namespace FubuFastPack.JqGrid
{
    public class GridRunner<TEntity, TService> : IGridRunner<TEntity, TService> where TEntity : DomainEntity
    {
        private readonly IObjectConverter _converter;
        private readonly IDisplayFormatter _formatter;
        private readonly IQueryService _queryService;
        private readonly TService _service;
        private readonly IUrlRegistry _urls;

        public GridRunner(IDisplayFormatter formatter, IUrlRegistry urls, IObjectConverter converter, TService service,
                          IQueryService queryService)
        {
            _formatter = formatter;
            _urls = urls;
            _converter = converter;
            _service = service;
            _queryService = queryService;
        }

        public TService Service
        {
            get { return _service; }
        }

        public GridResults RunGrid<T>(GridDefinition<T> grid, IGridDataSource<T> source, PagingOptions request)
        {
            applyCriteria(request, grid, source);

            var data = source.Fetch(request);
            var actions = grid.Columns.Select(col => col.CreateFiller(data, _formatter, _urls));
            var list = createEntityDtos(data, actions);

            // TODO -- needs some UT's
            return ApplyPaging(source, request, list);
        }

        private void applyCriteria<T>(PagingOptions paging, GridDefinition<T> grid, IGridDataSource<T> source)
        {
            var requests = paging.Criterion.Select(x =>
            {
                var expression = grid.ColumnFor(x.property).PropertyExpressionFor<T>();
                return new FilterRequest<T>(x, _converter, expression);
            });

            requests.Each(req => source.ApplyCriteria(req, _queryService));
        }

        private static List<EntityDTO> createEntityDtos(IGridData data, IEnumerable<Action<EntityDTO>> actions)
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
            var recordCount = source.TotalCount();
            var pageCount = (int) Math.Ceiling(recordCount/(double) paging.ResultsPerPage);

            if (pageCount < paging.Page)
            {
                paging.Page = pageCount;
            }

            return new GridResults{
                page = paging.Page,
                records = recordCount,
                total = pageCount,
                items = list
            };
        }
    }
}