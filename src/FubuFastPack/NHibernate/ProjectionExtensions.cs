using System.Collections.Generic;
using FubuCore.Util;
using FubuFastPack.Domain;
using NHibernate;
using NHibernate.SqlCommand;

namespace FubuFastPack.NHibernate
{
    public static class ProjectionExtensions
    {
        public static ICriteria AddAliases<T>(this IList<ProjectionColumn<T>> columns, ICriteria criteria) where T : DomainEntity
        {
            var aliasAndJoinTypeMap = new Cache<string, bool>();
            columns.Each(c => c.AddAlias(aliasAndJoinTypeMap));
            aliasAndJoinTypeMap.Each((alias, useOuterJoin) =>
            {
                JoinType joinType = useOuterJoin ? JoinType.LeftOuterJoin : JoinType.InnerJoin;
                criteria = criteria.CreateAlias(alias, alias, joinType);
            });

            return criteria;
        }
    }
}