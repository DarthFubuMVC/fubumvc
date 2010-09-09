using System;
using System.Linq.Expressions;
using FubuMVC.Core.View;
using FubuMVC.UI.Forms;
using FubuMVC.UI.Security;
using FubuMVC.UI.Tags;

namespace FubuMVC.UI
{


    public static class ShowEditFieldExpressions
    {
        public static FormLineExpression<T> Show<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression) where T : class
        {
            var tags = page.Tags();
            var request = tags.GetRequest(expression);
            var accessRight = page.Get<IFieldAccessService>().RightsFor(request);

            return new FormLineExpression<T>(tags, tags.NewFieldLayout(), request).Access(accessRight);
        }

        public static FormLineExpression<T> Edit<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression) where T : class
        {
            return page.Show(expression).Editable(true);
        }
    }
}