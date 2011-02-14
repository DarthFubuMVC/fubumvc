using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
using FubuMVC.Core.Urls;

namespace FubuFastPack.JqGrid
{
    public class GridRunner<TEntity, TService> : IGridRunner<TEntity, TService> where TEntity : DomainEntity
    {
        private readonly IObjectConverter _converter;
        private readonly IDisplayFormatter _formatter;
        private readonly IQueryService _queryService;
        private readonly IEnumerable<IDataRestriction<TEntity>> _restrictions;
        private readonly TService _service;
        private readonly IUrlRegistry _urls;

        public GridRunner(IDisplayFormatter formatter, IUrlRegistry urls, IObjectConverter converter, TService service,
                          IQueryService queryService, IEnumerable<IDataRestriction<TEntity>> restrictions)
        {
            _formatter = formatter;
            _urls = urls;
            _converter = converter;
            _service = service;
            _queryService = queryService;
            _restrictions = restrictions;
        }

        public TService Service
        {
            get { return _service; }
        }

        public GridResults RunGrid(GridDefinition<TEntity> grid, IGridDataSource<TEntity> source, GridDataRequest request)
        {
            if (request.ResultsPerPage > grid.MaxCount)
            {
                request.ResultsPerPage = grid.MaxCount;
            }

            grid.SortBy.ApplyDefaultSorting(request);
            
            applyCriteria(request, grid, source);
            applyRestrictions(source);

            var data = source.Fetch(request);
            var actions = grid.Columns.Select(col => col.CreateDtoFiller(data, _formatter, _urls));
            var list = createEntityDtos(data, actions);

            // TODO -- needs some UT's
            return ApplyPaging(source, request, list);
        }

        private void applyRestrictions(IGridDataSource<TEntity> source)
        {
            source.ApplyRestrictions(filter => _restrictions.Each(r => r.Apply(filter)));
        }

        public int GetCount(IGridDataSource<TEntity> source)
        {
            applyRestrictions(source);
            return source.TotalCount();
        }

        private void applyCriteria<T>(GridDataRequest paging, GridDefinition<T> grid, IGridDataSource<T> source)
            where T : DomainEntity
        {
            var requests = paging.Criterion.Select(x =>
            {
                var expression = grid.PropertyExpressionFor(x.property);
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

        public GridResults ApplyPaging<T>(IGridDataSource<T> source, GridDataRequest paging, List<EntityDTO> list) where T : DomainEntity
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