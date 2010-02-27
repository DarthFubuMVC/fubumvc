using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.View.WebForms
{
    public class RenderPartialExpression<VIEWMODEL>
        where VIEWMODEL : class
    {
        private readonly IFubuPage _parentPage;
        private bool _multiMode;
        private object _partialModel;
        private Action<StringBuilder> _multiModeAction;
        private string _prefix;
        private readonly IPartialRenderer _renderer;
        private readonly IFubuRequest _request;
        private IFubuPage _partialView;
        private bool _shouldDisplay = true;

        public RenderPartialExpression(IFubuPage parentPage, IPartialRenderer renderer, IFubuRequest request)
        {
            _renderer = renderer;
            _request = request;
            _parentPage = parentPage;
        }

        public RenderPartialExpression<VIEWMODEL> If(bool display)
        {
            _shouldDisplay = display;
            return this;
        }

        public RenderPartialExpression<VIEWMODEL> Using<PARTIALVIEW>()
            where PARTIALVIEW : IFubuPage
        {
            return Using<PARTIALVIEW>(null);
        }

        public RenderPartialExpression<VIEWMODEL> Using(Type partialViewType)
        {
            if (partialViewType.IsConcreteTypeOf<IFubuPage>())
                _partialView = _renderer.CreateControl(partialViewType);
            return this;
        }

        public RenderPartialExpression<VIEWMODEL> Using<PARTIALVIEW>(Action<PARTIALVIEW> optionAction)
            where PARTIALVIEW : IFubuPage
        {
            _partialView = _renderer.CreateControl(typeof(PARTIALVIEW));

            if (optionAction != null)
            {
                optionAction((PARTIALVIEW)_partialView);
            }

            return this;
        }

        public RenderPartialExpression<VIEWMODEL> For<PARTIALVIEWMODEL>(Expression<Func<VIEWMODEL, PARTIALVIEWMODEL>> expression)
            where PARTIALVIEWMODEL : class
        {
            SetupModelFromAccessor(ReflectionHelper.GetAccessor(expression), _request.Get<VIEWMODEL>());

            return this;
        }

        public RenderPartialExpression<VIEWMODEL> WithoutPrefix()
        {
            _prefix = string.Empty;
            return this;
        }


        public RenderPartialExpression<VIEWMODEL> For(object model)
        {
            _partialModel = model;
            _prefix = string.Empty;

            return this;
        }

        public RenderPartialExpression<VIEWMODEL> ForEachOf<PARTIALVIEWMODEL>(Expression<Func<VIEWMODEL, IEnumerable<PARTIALVIEWMODEL>>> expression)
            where PARTIALVIEWMODEL : class
        {
            SetupModelFromAccessor(ReflectionHelper.GetAccessor(expression), _request.Get<VIEWMODEL>());

            _multiModeAction = b => RenderMultiplePartials(b, (IEnumerable<PARTIALVIEWMODEL>) _partialModel);

            _multiMode = true;

            return this;
        }

        public RenderPartialExpression<VIEWMODEL> ForEachOf<PARTIALMODEL>(IEnumerable<PARTIALMODEL> modelList)
        {
            _partialModel = modelList;

            _multiMode = true;

            return this;
        }

        private void SetupModelFromAccessor(Accessor accessor, VIEWMODEL viewmodel)
        {
            if (viewmodel != null)
            {
                _partialModel = accessor.GetValue(viewmodel);
            }

            _prefix = accessor.Name;
        }

        public string RenderMultiplePartials()
        {
            var builder = new StringBuilder();

            _multiModeAction(builder);

            return builder.ToString();
        }

        public void RenderMultiplePartials<PARTIALVIEWMODEL>(StringBuilder builder, IEnumerable<PARTIALVIEWMODEL> list) 
            where PARTIALVIEWMODEL : class
        {
            list.Each(m =>
                      {
                          var output = _renderer.Render<PARTIALVIEWMODEL>(_partialView, m, _prefix);
                          builder.Append(output);
                      });
        }

        public override string ToString()
        {
            if (!_shouldDisplay) return "";

            return !_multiMode ? _renderer.Render(_parentPage, _partialView, _partialModel, _prefix) : RenderMultiplePartials();
        }
    }
}