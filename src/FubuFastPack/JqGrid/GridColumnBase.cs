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

        protected GridColumnBase(Accessor accessor, Expression<Func<T, object>> expression)
        {
            _accessor = accessor;
            _expression = expression;
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

        public void OverrideHeader(StringToken token)
        {
            _header = token;
        }

        public string GetHeader()
        {
            if (_header != null) return _header.ToString();

            return LocalizationManager.GetHeader(_expression);
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