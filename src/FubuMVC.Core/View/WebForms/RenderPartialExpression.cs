using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.View.WebForms
{
    public class RenderPartialExpression<TViewModel>
        where TViewModel : class
    {
        private readonly IFubuPage _parentPage;
        private Action<StringBuilder> _multiModeAction;
        private string _prefix;
        private readonly IPartialRenderer _renderer;
        private readonly IFubuRequest _request;
        private IFubuPage _partialView;
        private bool _shouldDisplay = true;
        private Func<string> _renderAction;



        public RenderPartialExpression(IFubuPage parentPage, IPartialRenderer renderer, IFubuRequest request)
        {
            _renderer = renderer;
            _request = request;
            _parentPage = parentPage;
        }

        public RenderPartialExpression<TViewModel> If(bool display)
        {
            _shouldDisplay = display;
            return this;
        }

        public RenderPartialExpression<TViewModel> Using<TPartialView>()
            where TPartialView : IFubuPage
        {
            return Using<TPartialView>(null);
        }

        public RenderPartialExpression<TViewModel> Using<TPartialView>(Action<TPartialView> optionAction)
            where TPartialView : IFubuPage
        {
            _partialView = _renderer.CreateControl(typeof(TPartialView));

            if (optionAction != null)
            {
                optionAction((TPartialView)_partialView);
            }

            return this;
        }



        public RenderPartialExpression<TViewModel> WithoutPrefix()
        {
            _prefix = string.Empty;
            return this;
        }


        public RenderPartialExpression<TViewModel> For<T>(T model) where T : class
        {
            _renderAction = () => _renderer.Render<T>(_parentPage, _partialView, model, _prefix);
            _prefix = string.Empty;

            return this;
        }

        public RenderPartialExpression<TViewModel> For<T>(Expression<Func<TViewModel, T>> expression)
            where T : class
        {
            Accessor accessor = ReflectionHelper.GetAccessor(expression);
            var viewmodel = _request.Get<TViewModel>();
            if (viewmodel != null)
            {
                var model = accessor.GetValue(viewmodel) as T;
                _renderAction = () => _renderer.Render<T>(_parentPage, _partialView, model, _prefix);
            }

            _prefix = accessor.Name;

            return this;
        }

        public RenderPartialExpression<TViewModel> ForEachOf<TPartialViewModel>(Expression<Func<TViewModel, IEnumerable<TPartialViewModel>>> expression)
            where TPartialViewModel : class
        {
            var viewmodel = _request.Get<TViewModel>();
            var accessor = ReflectionHelper.GetAccessor(expression);
            IEnumerable<TPartialViewModel> models = new TPartialViewModel[0];
            if (viewmodel != null)
            {
                models = accessor.GetValue(viewmodel) as IEnumerable<TPartialViewModel> ?? new TPartialViewModel[0];
            }

            _prefix = accessor.Name;

            return ForEachOf(models);
        }

        public RenderPartialExpression<TViewModel> ForEachOf<TPartialModel>(IEnumerable<TPartialModel> modelList) where TPartialModel : class
        {
            _multiModeAction = b => renderMultiplePartials(b, modelList);

            return this;
        }


        public string RenderMultiplePartials()
        {
            var builder = new StringBuilder();

            _multiModeAction(builder);

            return builder.ToString();
        }

        private void renderMultiplePartials<TPartialViewModel>(StringBuilder builder, IEnumerable<TPartialViewModel> list) 
            where TPartialViewModel : class
        {
            list.Each(m =>
            {
                var output = _renderer.Render(_partialView, m, _prefix);
                builder.Append(output);
            });
        }

        public override string ToString()
        {
            if (!_shouldDisplay) return "";

            return _multiModeAction != null 
                ? RenderMultiplePartials()
                : _renderAction();
        }
    }
}