using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuFastPack.Domain;

namespace FubuFastPack.Querying
{
    public class SingleEntityFilter<T> : IDataSourceFilter<T> where T : DomainEntity
    {
        private readonly T _domainEntity;

        public SingleEntityFilter(T domainEntity)
        {
            CanView = true;
            _domainEntity = domainEntity;
            DenyingRestriction = null;
        }

        public void WhereEqual(Expression<Func<T, object>> property, object value)
        {
            if (!CanView) return;
            var entityValue = ReflectionHelper.GetAccessor(property).GetValue(_domainEntity);

            if (ReferenceEquals(entityValue, null))
            {
                if (!ReferenceEquals(value, null)) CanView = false;
            }
            else
            {
                if (!entityValue.Equals(value)) CanView = false;
            }
        }

        public void WhereNotEqual(Expression<Func<T, object>> property, object value)
        {
            if (!CanView) return;
            var entityValue = ReflectionHelper.GetAccessor(property).GetValue(_domainEntity);
            if (ReferenceEquals(entityValue, null))
            {
                if (ReferenceEquals(value, null)) CanView = false;
            }
            else
            {
                if (entityValue.Equals(value)) CanView = false;
            }
        }

        public void ApplyRestriction(IDataRestriction<T> restriction)
        {
            restriction.Apply(this);
            if (!CanView && DenyingRestriction == null) DenyingRestriction = restriction;
        }

        // could be useful for diagnostics
        public IDataRestriction<T> DenyingRestriction { get; set; }

        public bool CanView { get; private set; }
    }
}