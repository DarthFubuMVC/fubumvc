using System;
using System.Linq.Expressions;
using FubuMVC.Core.View;
using FubuMVC.UI.Forms;
using FubuMVC.UI.Security;
using FubuMVC.UI.Tags;

namespace FubuMVC.UI
{
    public class FormLineExpressionBuilder<T> where T : class
    {
        private readonly ITagGenerator<T> _tags;
        private readonly IFieldAccessService _fieldAccess;

        public FormLineExpressionBuilder(ITagGenerator<T> tags, IFieldAccessService fieldAccess)
        {
            _tags = tags;
            _fieldAccess = fieldAccess;
        }

        public FormLineExpression<T> Build(Expression<Func<T, object>> expression)
        {
            var request = _tags.GetRequest(expression);
            var accessRight = _fieldAccess.RightsFor(request);
            return new FormLineExpression<T>(_tags, _tags.NewFieldLayout(), request).Access(accessRight);
        }
    }


    public static class ShowEditFieldExpressions
    {
        public static FormLineExpression<T> Show<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression) where T : class
        {
            return page.Get<FormLineExpressionBuilder<T>>().Build(expression);
        }

        public static FormLineExpression<T> Edit<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression) where T : class
        {
            return page.Show(expression).Editable(true);
        }
    }
}