using System;
using System.Linq.Expressions;

namespace FubuMVC.Core.Http.Scenarios
{
    public interface IUrlExpression
    {
        void Action<T>(Expression<Action<T>> expression);
        void Url(string relativeUrl);
        void Input<T>(T input = null) where T : class;
    }
}