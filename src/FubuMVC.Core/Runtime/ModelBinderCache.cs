using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Models;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Runtime
{
    public class ModelBinderCache : IModelBinderCache
    {
        private readonly IList<IModelBinder> _binders = new List<IModelBinder>();
        private readonly Cache<Type, IModelBinder> _cache = new Cache<Type,IModelBinder>();

        public ModelBinderCache(IEnumerable<IModelBinder> binders, IPropertyBinderCache propertyBinders, ITypeDescriptorCache types)
        {
            _binders.AddRange(binders);
            _binders.Add(new StandardModelBinder(propertyBinders, types));

            // TODO -- specific message if model binder cannot be found
            _cache.OnMissing = type => _binders.First(x => x.Matches(type));
        }

        public IModelBinder BinderFor(Type modelType)
        {
            return _cache[modelType];
        }
    }
}