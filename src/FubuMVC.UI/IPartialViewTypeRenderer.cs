using System;
using System.Collections.Generic;
using FubuMVC.Core.View;

namespace FubuMVC.UI
{
    public interface IPartialViewTypeRenderer
    {
        PartialViewTypeExpression For<TPartialModel>();
        Type GetPartialViewTypeFor<TPartialModel>();
        bool HasPartialViewTypeFor<TPartialModel>();
    }

    public class PartialViewTypeRenderer : IPartialViewTypeRenderer
    {
        private readonly IDictionary<Type, PartialViewTypeExpression> _viewModelTypes = new Dictionary<Type, PartialViewTypeExpression>();
        public PartialViewTypeExpression For<TPartialModel>()
        {
            return new PartialViewTypeExpression(this, typeof(TPartialModel));
        }

        public Type GetPartialViewTypeFor<TPartialModel>()
        {
            return _viewModelTypes[typeof(TPartialModel)].RenderType();
        }

        public bool HasPartialViewTypeFor<TPartialModel>()
        {
            return _viewModelTypes.ContainsKey(typeof(TPartialModel));
        }

        public void Register(Type modelType, PartialViewTypeExpression expression)
        {
            _viewModelTypes.Add(modelType, expression);
        }
    }

    public class PartialViewTypeExpression
    {
        private readonly PartialViewTypeRenderer _typeRenderer;
        private readonly Type _modelType;
        private Type _partialView;

        public PartialViewTypeExpression(PartialViewTypeRenderer typeRenderer, Type modelType)
        {
            _typeRenderer = typeRenderer;
            _modelType = modelType;
        }

        public PartialViewTypeExpression Use<TPartialView>()
            where TPartialView : IFubuPage
        {
            _partialView = typeof(TPartialView);
            _typeRenderer.Register(_modelType, this);
            return this;
        }

        public Type RenderType()
        {
            return _partialView;
        }
    }
}