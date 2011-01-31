using System;
using System.Linq.Expressions;
using FubuLocalization;

namespace FubuFastPack.Querying
{
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
}