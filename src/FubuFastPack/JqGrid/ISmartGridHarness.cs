using System;
using System.Collections.Generic;
using FubuFastPack.Domain;
using FubuFastPack.Querying;

namespace FubuFastPack.JqGrid
{
    public interface ISmartGridHarness
    {
        Type GridType { get; }
        GridViewModel BuildGridModel(IEnumerable<IGridPolicy> gridPolicies);

        string GetQuerystring();
        void RegisterArguments(params object[] arguments);
        string HeaderText();
        int Count();

        int Count<TEntity>(IDataRestriction<TEntity> restriction) where TEntity : DomainEntity;

        Type EntityType();
        Guid IdOfFirstResult();
    }
}