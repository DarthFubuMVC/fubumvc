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
            if (_accessor != null) PropertyName = _accessor.ToPropertyName();
        }

        public string PropertyName { get; set; }
        public bool OuterJoin { get; set; }

        public virtual void AddProjection(ProjectionList projections)
        {
            var projection = Projections.Property(_accessor.ToPropertyName()).As(_accessor.Name);
            projections.Add(projection);
        }

        public virtual void AddAlias(Cache<string, bool> aliasAndJoinTypeMap)
        {
            var key = _accessor.PropertyNames[0];

            if (_accessor.PropertyNames.Length <= 1 || aliasAndJoinTypeMap.Has(key)) return;

            aliasAndJoinTypeMap[key] = OuterJoin;
        }
    }

    public class DiscriminatorProjectionColumn<T> : ProjectionColumn<T> where T : DomainEntity
    {
        public DiscriminatorProjectionColumn() : base(null as Accessor)
        {
        }

        public override void AddProjection(ProjectionList projections)
        {
            projections.Add(Projections.Property("class"));
        }

        public override void AddAlias(Cache<string, bool> aliasAndJoinTypeMap)
        {
            // no-op
        }
    }
}