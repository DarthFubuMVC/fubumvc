using System;
using System.Linq.Expressions;
using FubuFastPack.Domain;
using NHibernate;

namespace FubuFastPack.NHibernate
{
    public class DtoProjection<TEntity, TDto>
        where TEntity : DomainEntity
        where TDto : class, new()
    {
        private readonly Projection<TEntity> _projection;
        private readonly ProjectionRequestData _data = new ProjectionRequestData();

        public DtoProjection(ISession session)
        {
            _projection = new Projection<TEntity>(session);
        }

        public PropertyExpression From(Expression<Func<TEntity, object>> expression)
        {
            return new PropertyExpression(this, expression);
        }


        public class PropertyExpression
        {
            private readonly DtoProjection<TEntity, TDto> _parent;
            private ProjectionColumn<TEntity> _fromColumn;

            public PropertyExpression(DtoProjection<TEntity, TDto> parent, Expression<Func<TEntity, object>> from)
            {
                _parent = parent;
                _fromColumn = parent._projection.AddColumn(from);
            }

            public void To(Expression<Func<TDto, object>> expression)
            {
                _parent._data.AddPropertyName(expression.Name);
            }
        }
    }
}