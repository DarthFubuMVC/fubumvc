using System;
using System.Collections.Generic;

namespace FubuMVC.WebForms
{
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
}