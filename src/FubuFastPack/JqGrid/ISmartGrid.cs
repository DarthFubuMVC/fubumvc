using System;
using System.Collections.Generic;
using FubuFastPack.Domain;
using FubuFastPack.Querying;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.JqGrid
{
    public interface ISmartGrid
    {
        IGridDefinition Definition { get; }

        // TODO -- let's move this inside.  Could use, *gasp* Setter injection here
        GridResults Invoke(IServiceLocator services, GridDataRequest request);

        IEnumerable<FilteredProperty> AllFilteredProperties(IQueryService queryService);

        IEnumerable<IDictionary<string, object>> ToColumnModel();
        int Count(IServiceLocator services);
        string GetHeader();
        void DisableLinks();
        IEnumerable<Criteria> InitialCriteria();

        Type EntityType { get; }

        void ApplyPolicies(IEnumerable<IGridPolicy> policies);

        SortOrder SortOrder();
    }

    public interface ISmartGrid<T> : ISmartGrid where T : DomainEntity
    {
        int Count(IServiceLocator services, IDataRestriction<T> restriction);
    }
}