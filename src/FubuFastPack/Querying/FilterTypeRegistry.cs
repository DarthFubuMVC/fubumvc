using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuLocalization;

namespace FubuFastPack.Querying
{
    public static class FilterTypeRegistry
    {
        private static readonly Cache<Type, IList<IFilterType>> _filterCache =
            new Cache<Type, IList<IFilterType>>(key => new List<IFilterType>());

        static FilterTypeRegistry()
        {
            ResetAll();
        }

        public static FilterTypeRegisterExpression RegisterFilter(IFilterType filter)
        {
            return new FilterTypeRegisterExpression(filter);
        }

        public static void RegisterFilterForType(IFilterType filterType, Type filterableType)
        {
            _filterCache[filterableType].Add(filterType);
        }

        public static IEnumerable<IFilterType> GetFiltersFor<T>()
        {
            return GetFiltersFor(typeof(T));
        }

        public static IEnumerable<IFilterType> GetFiltersFor(Type filterableType)
        {
            return _filterCache[filterableType];
        }

        public static void ClearAll()
        {
            _filterCache.ClearAll();
        }

        public static void ResetAll()
        {
            ClearAll();

            var numericTypes = new[]
                                    {
                                        typeof (byte),
                                        typeof (short),
                                        typeof (int),
                                        typeof (long),
                                        typeof (sbyte),
                                        typeof (ushort),
                                        typeof (uint),
                                        typeof (ulong),
                                        typeof (byte?),
                                        typeof (short?),
                                        typeof (int?),
                                        typeof (long?),
                                        typeof (sbyte?),
                                        typeof (ushort?),
                                        typeof (uint?),
                                        typeof (ulong?)
                                    };
            var dateTypes = new[]
                                {
                                    typeof (DateTime),
                                    typeof (DateTime?),
                                    typeof (DateTimeOffset),
                                    typeof (DateTimeOffset?)
                                };
            var booleanTypes = new[]
                                   {
                                       typeof (bool),
                                       typeof (bool?)
                                   };
            #region registrations
            
            RegisterFilter(new StringFilterType { Key = OperatorKeys.STARTSWITH, IgnoreCase = true, StringMethod = s => s.StartsWith("") })
                .ForType<string>();
            RegisterFilter(new StringFilterType { Key = OperatorKeys.CONTAINS, IgnoreCase = true, StringMethod = s => s.Contains("") })
                .ForType<string>();

            RegisterFilter(new StringFilterType { Key = OperatorKeys.ENDSWITH, IgnoreCase = true, StringMethod = s => s.EndsWith("") })
                .ForType<string>();

            RegisterFilter(new BinaryFilterType { Key = OperatorKeys.EQUAL, FilterExpressionType = ExpressionType.Equal })
                .ForTypes(numericTypes)
                .ForType<string>();

            RegisterFilter(new BinaryFilterType { Key = OperatorKeys.NOTEQUAL, FilterExpressionType = ExpressionType.NotEqual })
                .ForTypes(numericTypes)
                .ForType<string>();

            RegisterFilter(new BinaryFilterType { Key = OperatorKeys.GREATERTHAN, FilterExpressionType = ExpressionType.GreaterThan })
                .ForTypes(numericTypes);

            RegisterFilter(new BinaryFilterType { Key = OperatorKeys.GREATERTHANOREQUAL, FilterExpressionType = ExpressionType.GreaterThanOrEqual })
                .ForTypes(numericTypes);

            RegisterFilter(new BinaryFilterType { Key = OperatorKeys.LESSTHAN, FilterExpressionType = ExpressionType.LessThan })
                .ForTypes(numericTypes);

            RegisterFilter(new BinaryFilterType { Key = OperatorKeys.LESSTHANOREQUAL, FilterExpressionType = ExpressionType.LessThanOrEqual })
                .ForTypes(numericTypes);



            //RegisterFilter(new BinaryFilterType() { Key = OperatorKeys.WITHIN_X_DAYS, Modifier = DateTimeFilterTypeModifiers.DaysAgo, FilterExpressionType = ExpressionType.GreaterThanOrEqual })
            //    .ForTypes(dateTypes);

            //RegisterFilter(new BinaryFilterType() { Key = OperatorKeys.AFTER_X_DAYS, Modifier = DateTimeFilterTypeModifiers.DaysAgo, FilterExpressionType = ExpressionType.LessThanOrEqual })
            //    .ForTypes(dateTypes);

            //RegisterFilter(new BinaryFilterType() { InputStyle = typeof(DateTime).Name, Key = OperatorKeys.AFTER_DATE, Modifier = DateTimeFilterTypeModifiers.DateToUtc, FilterExpressionType = ExpressionType.GreaterThanOrEqual })
            //    .ForTypes(dateTypes);

            //RegisterFilter(new BinaryFilterType() { InputStyle = typeof(DateTime).Name, Key = OperatorKeys.BEFORE_DATE, Modifier = DateTimeFilterTypeModifiers.DateToUtc, FilterExpressionType = ExpressionType.LessThanOrEqual })
            //    .ForTypes(dateTypes);

            // EQUAL/NOTEQUAL registered in a different order for date types
            RegisterFilter(new BinaryFilterType { Key = OperatorKeys.EQUAL, FilterExpressionType = ExpressionType.Equal })
                .ForTypes(booleanTypes)
                .ForType<Guid>();

            RegisterFilter(new BinaryFilterType { Key = OperatorKeys.NOTEQUAL, FilterExpressionType = ExpressionType.NotEqual })
                .ForTypes(booleanTypes)
                .ForType<Guid>();

            #endregion
        }
    }

    public interface IFilterType
    {
        StringToken Key { get; }
        Expression GetExpression(Expression memberAccessExpression, Expression valueExpression);
        Func<string, string> Modifier { get; set; }
        string InputStyle { get; set; }
    }


    public class BinaryFilterType : IFilterType
    {
        public BinaryFilterType()
        {
            Modifier = s => s;
        }

        public ExpressionType FilterExpressionType { get; set; }

        public StringToken Key { get; set; }

        public Expression GetExpression(Expression memberAccessExpression, Expression valueExpression)
        {
            return Expression.MakeBinary(FilterExpressionType, memberAccessExpression, valueExpression);
        }

        public Func<string, string> Modifier { get; set; }

        public string InputStyle
        {
            get;
            set;
        }
    }

    public class StringFilterType : IFilterType
    {
        private MethodInfo _stringMethodInfo;
        private static MethodInfo _toUpperMethodCached;
        private static MethodInfo ToUpperMethod
        {
            get
            {
                if (_toUpperMethodCached == null)
                {
                    _toUpperMethodCached = ReflectionHelper.GetMethod<string>(s => s.ToUpper());
                }
                return _toUpperMethodCached;
            }
        }

        public Expression<Func<string, bool>> StringMethod { get; set; }
        public bool IgnoreCase { get; set; }
        public StringToken Key { get; set; }

        public virtual Expression GetExpression(Expression memberAccessExpression, Expression valueExpression)
        {
            //NHibernate.Linq currently throws an exception when dealing with the ToUpper call on a partial match
            //We will disable IgnoreCase for now, and just rely on SQL Server to do the case-insensitive matching
            IgnoreCase = false;

            if (_stringMethodInfo == null)
            {
                _stringMethodInfo = ReflectionHelper.GetMethod(StringMethod);
            }
            var valueToCheck = IgnoreCase ? Expression.Call(valueExpression, ToUpperMethod) : valueExpression;

            var callee = memberAccessExpression;

            if (IgnoreCase)
            {
                callee = Expression.Call(memberAccessExpression, ToUpperMethod);
            }

            return Expression.Call(callee, _stringMethodInfo, valueToCheck);
        }

        public Func<string, string> Modifier
        {
            get { return s => s; }
            set { throw new NotImplementedException(); }
        }

        public string InputStyle
        {
            get;
            set;
        }
    }

    public static class FilterTypeExtensions
    {
        public static string Operator(this IFilterType filter)
        {
            return filter.Key.Key;
        }

        public static OperatorDTO ToDTO(this IFilterType filter)
        {
            return new OperatorDTO(){
                display = filter.Key.ToString(),
                value = filter.Key.Key
            };
        }
    }

    //public static class DateTimeFilterTypeModifiers
    //{
    //    private static Func<TimeZoneInfo> _currentTimezone = () => DovetailPrincipal.CurrentTimezone;

    //    public static void Live()
    //    {
    //        _currentTimezone = () => DovetailPrincipal.CurrentTimezone;
    //    }

    //    public static void Stub(Func<TimeZoneInfo> timeZoneInfo)
    //    {
    //        _currentTimezone = timeZoneInfo;
    //    }

    //    public static string DaysAgo(string numberOfDays)
    //    {
    //        var convertedNumberOfDays = 0;
    //        //needed for integration test.
    //        Int32.TryParse(numberOfDays, out convertedNumberOfDays);
    //        return _currentTimezone().StartOfTodayInUtc().AddDays(0 - convertedNumberOfDays).ToString();
    //    }

    //    public static string DateToUtc(string dateTime)
    //    {
    //        return DateTime.Parse(dateTime).ToUniversalTime(_currentTimezone()).ToString();
    //    }
    //}

    public class FilterTypeRegisterExpression
    {
        private readonly IFilterType _filter;

        public FilterTypeRegisterExpression(IFilterType filter)
        {
            _filter = filter;
        }

        public FilterTypeRegisterExpression ForType<T>()
        {
            FilterTypeRegistry.RegisterFilterForType(_filter, typeof(T));
            return this;
        }

        public FilterTypeRegisterExpression ForTypes(params Type[] filterableTypes)
        {
            foreach (var filterableType in filterableTypes)
            {
                FilterTypeRegistry.RegisterFilterForType(_filter, filterableType);
            }

            return this;
        }
    }
}