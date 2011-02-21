using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuFastPack.Querying;
using FubuLocalization;

namespace FubuFastPack.JqGrid
{
    public abstract class GridColumnBase<T, TColumnType> where TColumnType : class
    {
        private readonly Accessor _accessor;
        private Accessor _headerAccessor;
        private readonly Expression<Func<T, object>> _expression;
        private StringToken _header;

        protected GridColumnBase(Expression<Func<T, object>> expression)
        {
            _accessor = expression.ToAccessor();
            _headerAccessor = _accessor;
            _expression = expression;
        }

        protected GridColumnBase(Accessor accessor)
        {
            _accessor = accessor;
            _headerAccessor = accessor;
            _expression = accessor.ToExpression<T>();
        }

        public Accessor Accessor
        {
            get { return _accessor; }
        }

        public Expression<Func<T, object>> Expression
        {
            get { return _expression; }
        }

        public TColumnType Filterable(bool filterable)
        {
            IsFilterable = filterable;
            return this as TColumnType;
        }

        public bool IsFilterable { get; set; }
        public bool IsSortable { get; set; }

        public TColumnType OuterJoin()
        {
            IsOuterJoin = true;
            return this as TColumnType;
        }

        public bool IsOuterJoin { get; set; }

        public TColumnType HeaderFrom(StringToken header)
        {
            _header = header;

            return this as TColumnType;
        }

        public TColumnType HeaderFrom(Expression<Func<T, object>> property)
        {
            _headerAccessor = property.ToAccessor();

            return this as TColumnType;
        }

        public TColumnType Sortable(bool isSortable)
        {
            IsSortable = isSortable;
            return this as TColumnType;
        }

        // TODO -- get rid of this
        public string GetHeader()
        {
            if (_header != null) return _header.ToString();

            return LocalizationManager.GetHeader(_headerAccessor.InnerProperty);
        }

        public IEnumerable<FilteredProperty> FilteredProperties()
        {
            if (!IsFilterable) yield break;

            yield return new FilteredProperty(){
                Accessor = Accessor,
                Header = GetHeader()
            };
        }

    }
}