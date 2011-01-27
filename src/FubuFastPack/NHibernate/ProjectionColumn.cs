using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuFastPack.Domain;
using NHibernate.Criterion;

namespace FubuFastPack.NHibernate
{
    public class ProjectionColumn<T> where T : DomainEntity
    {
        protected Accessor _accessor;

        public Accessor PropertyAccessor
        {
            get { return _accessor; }
        }

        public ProjectionColumn(Expression<Func<T, object>> expression)
            : this(expression.ToAccessor())
        {

        }

        public ProjectionColumn(Accessor accessor)
        {
            _accessor = accessor;
            if (_accessor != null) PropertyName = _accessor.Name;
        }

        public string PropertyName { get; set; }
        public bool OuterJoin { get; set; }

        public virtual void AddProjection(ProjectionList projections)
        {
            string propertyName = String.Join(".", _accessor.PropertyNames);
            var projection = Projections.Property(propertyName).As(_accessor.Name);
            projections.Add(projection);
        }

        public virtual void AddAlias(Cache<string, bool> aliasAndJoinTypeMap)
        {
            var key = _accessor.PropertyNames[0];

            if (_accessor.PropertyNames.Length <= 1 || aliasAndJoinTypeMap.Has(key)) return;

            aliasAndJoinTypeMap[key] = OuterJoin;
        }
    }
}