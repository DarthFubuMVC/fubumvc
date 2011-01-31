using System;
using System.Linq;
using FubuFastPack.Domain;
using FubuFastPack.NHibernate;
using FubuFastPack.Querying;

namespace FubuFastPack.JqGrid
{
    public class ProjectionDataSource<T> : IGridDataSource<T> 
        where T : DomainEntity
    {
        private readonly Projection<T> _projection;

        public ProjectionDataSource(Projection<T> projection)
        {
            _projection = projection;
        }

        public int TotalCount()
        {
            return _projection.Count();
        }

        public IGridData Fetch(PagingOptions options)
        {
            var records = _projection.ExecuteCriteriaWithProjection(options).Cast<object>().ToList();
            var accessors = _projection.SelectAccessors().ToList();

            return new ProjectionGridData(records, accessors);
        }

        public void ApplyCriteria(FilterRequest<T> request, IQueryService queryService)
        {
            throw new NotSupportedException();
        }
    }
}