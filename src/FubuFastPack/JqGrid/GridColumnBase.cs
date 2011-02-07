using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuFastPack.Querying;
using FubuLocalization;

namespace FubuFastPack.JqGrid
{
    public abstract class GridColumnBase<T>
    {
        private readonly Accessor _accessor;
        private readonly Expression<Func<T, object>> _expression;
        private StringToken _header;

        protected GridColumnBase(Expression<Func<T, object>> expression)
        {
            _accessor = expression.ToAccessor();
            _expression = expression;
        }

        protected GridColumnBase(Accessor accessor)
        {
            _accessor = accessor;
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

        public bool IsFilterable { get; set; }
        public bool IsSortable { get; set; }

        public void OverrideHeader(StringToken token)
        {
            _header = token;
        }

        public string GetHeader()
        {
            if (_header != null) return _header.ToString();

            return LocalizationManager.GetHeader(_accessor.InnerProperty);
        }

        public IEnumerable<FilterDTO> PossibleFilters(IQueryService queryService)
        {
            if (!IsFilterable) yield break;

            yield return new FilterDTO {
                display = GetHeader(),
                value = Accessor.Name,
                operators = queryService.FilterOptionsFor(Expression).Select(x => x.ToOperator()).ToArray()
            };
        }
    }
}