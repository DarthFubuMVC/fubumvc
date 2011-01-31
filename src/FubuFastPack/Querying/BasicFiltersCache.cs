using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection.Expressions;

namespace FubuFastPack.Querying
{
    public static class BasicFiltersCache
    {
        private static readonly Func<Type, bool> numbers = type => type.IsNumeric();
        private static readonly Func<Type, bool> booleans = type => type.IsBoolean();
        private static readonly Func<Type, bool> strings = type => type == typeof (string);

        private static readonly IList<IFilterHandler> _handlers = new List<IFilterHandler>();

        static BasicFiltersCache()
        {
            filterIs<EqualsPropertyOperation>(OperatorKeys.EQUAL, numbers, strings, booleans, typeOf<Guid>());
            filterIs<NotEqualPropertyOperation>(OperatorKeys.NOTEQUAL, numbers, strings, booleans, typeOf<Guid>());
            filterIs<GreaterThanPropertyOperation>(OperatorKeys.GREATERTHAN, numbers);
            filterIs<LessThanPropertyOperation>(OperatorKeys.LESSTHAN, numbers);
            filterIs<GreaterThanOrEqualPropertyOperation>(OperatorKeys.GREATERTHANOREQUAL, numbers);
            filterIs<LessThanOrEqualPropertyOperation>(OperatorKeys.LESSTHANOREQUAL, numbers);

            stringOp(OperatorKeys.STARTSWITH, s => s.StartsWith(""));
            stringOp(OperatorKeys.ENDSWITH, s => s.EndsWith(""));
            stringOp(OperatorKeys.CONTAINS, s => s.Contains(""));
        }

        public static IEnumerable<IFilterHandler> AllHandlers
        {
            get { return _handlers; }
        }

        private static Func<Type, bool> typeOf<T>()
        {
            return type => type == typeof (T);
        }

        private static void filterIs<T>(OperatorKeys key, params Func<Type, bool>[] typeFilters)
            where T : IPropertyOperation, new()
        {
            var handler = new BinaryFilterHandler<T>(key, typeFilters);
            _handlers.Add(handler);
        }

        private static void stringOp(OperatorKeys key, Expression<Func<string, bool>> method)
        {
            var handler = new StringFilterHandler(key, method);
            _handlers.Add(handler);
        }
    }
}