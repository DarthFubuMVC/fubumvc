using System;
using System.Linq;
using FubuCore.Reflection;

namespace FubuCore.Binding
{
    public class StandardModelBinder : IModelBinder
    {
        private readonly IPropertyBinderCache _propertyBinders;
        private readonly ITypeDescriptorCache _typeCache;

        public StandardModelBinder(IPropertyBinderCache propertyBinders, ITypeDescriptorCache typeCache)
        {
            _propertyBinders = propertyBinders;
            _typeCache = typeCache;
        }

        public bool Matches(Type type)
        {
            return type.GetConstructors().Count(x => x.GetParameters().Length == 0) == 1;
        }

        public object Bind(Type type, IBindingContext context)
        {
            var instance = Activator.CreateInstance(type);
            Bind(type, instance, context);

            return instance;
        }

        public void Bind(Type type, object instance, IBindingContext context)
        {
            context.ForObject(instance, () => populate(type, context));
        }

        public static IModelBinder Basic()
        {
            return
                new StandardModelBinder(
                    new PropertyBinderCache(new IPropertyBinder[0], new ValueConverterRegistry(new IConverterFamily[0]),
                                            new DefaultCollectionTypeProvider()),
                    new TypeDescriptorCache());
        }


        // Only exists for easier testing
        public void Populate(object target, IBindingContext context)
        {
            context.ForObject(target, () => populate(target.GetType(), context));
        }

        private void populate(Type type, IBindingContext context)
        {
            _typeCache.ForEachProperty(type, prop => { _propertyBinders.BinderFor(prop).Bind(prop, context); });
        }
    }
}