using System;
using System.Linq.Expressions;
using FubuLocalization;

namespace FubuFastPack.Querying
{
    public interface IFilterType
    {
        StringToken Key { get; }
        Expression GetExpression(Expression memberAccessExpression, Expression valueExpression);
        Func<string, string> Modifier { get; set; }
        string InputStyle { get; set; }
    }
}