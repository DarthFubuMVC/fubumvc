using System;
using System.Collections.Generic;

namespace FubuMVC.Core.View.WebForms
{
    public interface IPartialViewTypeRegistry
    {
        Type GetPartialViewTypeFor<TPartialModel>();
        bool HasPartialViewTypeFor<TPartialModel>();
        void Register(Type modelType, Type expression);
    }

    public class PartialViewTypeRegistry : IPartialViewTypeRegistry
    {
        private readonly IDictionary<Type, Type> _viewModelTypes = new Dictionary<Type, Type>();
        
        public Type GetPartialViewTypeFor<TPartialModel>()
        {
            if (HasPartialViewTypeFor<TPartialModel>())
                return _viewModelTypes[typeof(TPartialModel)];
            return null;
        }

        public bool HasPartialViewTypeFor<TPartialModel>()
        {
            return _viewModelTypes.ContainsKey(typeof(TPartialModel));
        }

        public void Register(Type modelType, Type viewType)
        {
            _viewModelTypes.Add(modelType, viewType);
        }
    }

    public class PartialViewTypeBindingExpression : IPartialViewTypeBindingExpression
    {
        private readonly IPartialViewTypeRegistry _typeRegistry;
        private readonly Type _modelType;

        public PartialViewTypeBindingExpression(IPartialViewTypeRegistry typeRegistry, Type modelType)
        {
            _typeRegistry = typeRegistry;
            _modelType = modelType;
        }

        public void Use<TPartialView>()
            where TPartialView : IFubuPage
        {
            _typeRegistry.Register(_modelType, typeof(TPartialView));
        }
    }

    public interface IPartialViewTypeBindingExpression
    {
        void Use<TPartialView>()
            where TPartialView : IFubuPage;
    }

    public interface IPartialViewTypeRegistrationExpression
    {
        IPartialViewTypeBindingExpression For<TPartialModel>();
    }

    public class PartialViewTypeRegistrationExpression : IPartialViewTypeRegistrationExpression
    {
        private readonly IPartialViewTypeRegistry _registry;

        public PartialViewTypeRegistrationExpression(IPartialViewTypeRegistry registry)
        {
            _registry = registry;
        }

        public IPartialViewTypeBindingExpression For<TPartialModel>()
        {
            return new PartialViewTypeBindingExpression(_registry, typeof(TPartialModel));
        }
    }
}