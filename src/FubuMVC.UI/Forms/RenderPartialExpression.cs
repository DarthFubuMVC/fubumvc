using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.View;
using FubuMVC.Core.View.WebForms;
using FubuMVC.UI.Tags;


namespace FubuMVC.UI.Forms
{
    public class RenderPartialExpression<TViewModel>
        where TViewModel : class
    {
        private readonly IFubuPage _parentPage;
        private Action<StringBuilder> _multiModeAction;
        private string _prefix;
        private readonly TViewModel _model;
        private readonly IPartialRenderer _renderer;
        private readonly ITagGenerator<TViewModel> _tagGenerator;
        private IFubuPage _partialView;
        private bool _shouldDisplay = true;
        private Func<string> _renderAction;
        private Accessor _accessor;
        private bool _renderListWrapper = true;
        private bool _renderItemWrapper = true;



        public RenderPartialExpression(TViewModel model, IFubuPage parentPage, IPartialRenderer renderer, ITagGenerator<TViewModel> tagGenerator)
        {
            _model = model;
            _renderer = renderer;
            _tagGenerator = tagGenerator;
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

        public RenderPartialExpression<TViewModel> Using(Type partialViewType)
        {
            if (partialViewType.IsConcreteTypeOf<IFubuPage>())
                _partialView = _renderer.CreateControl(partialViewType);
            return this;
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

        public RenderPartialExpression<TViewModel> WithoutListWrapper()
        {
            _renderListWrapper = false;
            return this;
        }

        public RenderPartialExpression<TViewModel> WithoutItemWrapper()
        {
            _renderItemWrapper = false;
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
            _accessor = ReflectionHelper.GetAccessor(expression);
            if (_model != null)
            {
                var model = _accessor.GetValue(_model) as T;
                _renderAction = () => _renderer.Render(_parentPage, _partialView, model, _prefix);
            }

            _prefix = _accessor.Name;

            return this;
        }

        public RenderPartialExpression<TViewModel> ForEachOf<TPartialViewModel>(Expression<Func<TViewModel, IEnumerable<TPartialViewModel>>> expression)
            where TPartialViewModel : class
        {
            _accessor = ReflectionHelper.GetAccessor(expression);
            IEnumerable<TPartialViewModel> models = new TPartialViewModel[0];
            if (_model != null)
            {
                models = _accessor.GetValue(_model) as IEnumerable<TPartialViewModel> ?? new TPartialViewModel[0];
            }

            _prefix = _accessor.Name;

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
            if (_renderListWrapper)
            {
                var before = _tagGenerator.BeforePartial(_tagGenerator.GetRequest(_accessor));
                builder.Append(before);
            }

            var render_multiple_count = list.Count();
            var current = 0;

            list.Each(m =>
            {
                if (_renderItemWrapper)
                {
                    var beforeEach = _tagGenerator.BeforeEachofPartial(_tagGenerator.GetRequest(_accessor), current, render_multiple_count);
                    builder.Append(beforeEach);
                }

                var output = _renderer.Render<TPartialViewModel>(_partialView, m, _prefix);
                builder.Append(output);

                if (_renderItemWrapper)
                {
                    var afterEach = _tagGenerator.AfterEachofPartial(_tagGenerator.GetRequest(_accessor), current, render_multiple_count);
                    builder.Append(afterEach);
                }
                current++;
            });

            if (_renderListWrapper)
            {
                var after = _tagGenerator.AfterPartial(_tagGenerator.GetRequest(_accessor));
                builder.Append(after);
            }
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