using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            IModelBinder binder = _cache[modelType];
            if (binder == null)
            {
                throw new FubuException(2200, 
                    "Could not determine an IModelBinder for input type {0}. No model binders matched on this type. The standard model binder requires a parameterless constructor for the model type. Alternatively, you could implement your own IModelBinder which can process this model type.",
                    modelType.AssemblyQualifiedName);
            }

            return binder;
        }
    }
}