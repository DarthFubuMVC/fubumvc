using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            _cache.OnMissing = type => _binders.FirstOrDefault(x => x.Matches(type));
        }

        public IModelBinder BinderFor(Type modelType)
        {
            Debug.WriteLine("Getting binder for " + modelType.FullName);
            IModelBinder binder = _cache[modelType];
            if (binder == null)
            {
                throw new FubuException(2200, "Could not determine an IModelBinder for input type {0}", modelType.AssemblyQualifiedName);
            }

            return binder;
        }
    }
}