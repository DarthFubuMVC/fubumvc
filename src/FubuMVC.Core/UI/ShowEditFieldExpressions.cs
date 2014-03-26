using System;
using System.Linq.Expressions;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Forms;
using FubuMVC.Core.UI.Security;
using FubuMVC.Core.View;
using FubuCore.Reflection;
using HtmlTags.Conventions;

namespace FubuMVC.Core.UI
{
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

    public class FormLineExpressionBuilder<T> where T : class
    {
        private readonly IElementGenerator<T> _generator;
        private readonly HtmlConventionLibrary _library;
        private readonly ActiveProfile _profile;
        private readonly IFieldAccessService _fieldAccessService;
        private readonly IFubuRequest _request;

        public FormLineExpressionBuilder(IElementGenerator<T> generator, HtmlConventionLibrary library, ActiveProfile profile, IFieldAccessService fieldAccessService, IFubuRequest request)
        {
            _generator = generator;
            _library = library;
            _profile = profile;
            _fieldAccessService = fieldAccessService;
            _request = request;
        }

        public FormLineExpression<T> Build(Expression<Func<T, object>> expression, T model = null)
        {
            var request = new ElementRequest(expression.ToAccessor()) {Model = model ?? _request.Get<T>()};

            var accessRight = _fieldAccessService.RightsFor(request);
            var chrome = _library.Get<IFieldChrome>(_profile.Name);

            return new FormLineExpression<T>(chrome, _generator, request).Access(accessRight);
        }
    }
}