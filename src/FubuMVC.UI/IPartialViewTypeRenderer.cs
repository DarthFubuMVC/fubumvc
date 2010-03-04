using System;
using System.Collections.Generic;
using FubuMVC.Core.View;

namespace FubuMVC.UI
{
    public interface IPartialViewTypeRenderer
    {
        IPartialViewTypeBindingExpression For<TPartialModel>();
        Type GetPartialViewTypeFor<TPartialModel>();
        bool HasPartialViewTypeFor<TPartialModel>();
        void Register(Type modelType, IPartialViewTypeExpression expression);
    }

    public class PartialViewTypeRenderer : IPartialViewTypeRenderer
    {
        private readonly IDictionary<Type, IPartialViewTypeExpression> _viewModelTypes = new Dictionary<Type, IPartialViewTypeExpression>();
        public IPartialViewTypeBindingExpression For<TPartialModel>()
        {
            return new PartialViewTypeBindingExpression(this, typeof(TPartialModel));
        }

        public Type GetPartialViewTypeFor<TPartialModel>()
        {
            return _viewModelTypes[typeof(TPartialModel)].RenderType();
        }

        public bool HasPartialViewTypeFor<TPartialModel>()
        {
            return _viewModelTypes.ContainsKey(typeof(TPartialModel));
        }

        public void Register(Type modelType, IPartialViewTypeExpression expression)
        {
            _viewModelTypes.Add(modelType, expression);
        }
    }

    public class PartialViewTypeBindingExpression : IPartialViewTypeBindingExpression
    {
        private readonly IPartialViewTypeRenderer _typeRenderer;
        private readonly Type _modelType;

        public PartialViewTypeBindingExpression(IPartialViewTypeRenderer typeRenderer, Type modelType)
        {
            _typeRenderer = typeRenderer;
            _modelType = modelType;
        }

        public void Use<TPartialView>()
            where TPartialView : IFubuPage
        {
            _typeRenderer.Register(_modelType, new PartialViewTypeExpression(typeof(TPartialView)));
        }
    }

    public interface IPartialViewTypeBindingExpression
    {
        void Use<TPartialView>()
            where TPartialView : IFubuPage;
    }

    public interface IPartialViewTypeExpression
    {
        Type RenderType();
    }

    public class PartialViewTypeExpression : IPartialViewTypeExpression
    {
        private readonly Type _partialView;

        public PartialViewTypeExpression(Type partialView)
        {
            _partialView = partialView;
        }

        public Type RenderType()
        {
            return _partialView;
        }
    }
}