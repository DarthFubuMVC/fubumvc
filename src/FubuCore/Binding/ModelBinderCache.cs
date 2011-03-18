using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuCore.Binding;
using FubuMVC.Core;

namespace FubuCore.Binding
{
    public class ModelBinderCache : IModelBinderCache
    {
        private readonly IList<IModelBinder> _binders = new List<IModelBinder>();
        private readonly Cache<Type, IModelBinder> _cache = new Cache<Type,IModelBinder>();

        public ModelBinderCache(IEnumerable<IModelBinder> binders, IPropertyBinderCache propertyBinders, ITypeDescriptorCache types)
        {
            // DO NOT put the standard model binder at top
            _binders.AddRange(binders.Where(x => !(x is StandardModelBinder)));
            _binders.Add(new StandardModelBinder(propertyBinders, types));

            _cache.OnMissing = type => _binders.FirstOrDefault(x => x.Matches(type));
        }

        public IModelBinder BinderFor(Type modelType)
        {
            return _cache[modelType];
        }
    }
}