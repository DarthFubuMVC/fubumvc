using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuLocalization;

namespace FubuFastPack.Querying
{
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
}