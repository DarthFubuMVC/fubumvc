using System;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Extensibility;

namespace FubuFastPack.Crud.Properties
{
    public static class PropertyUtility
    {
        public static Accessor FindPropertyByName<TEntity>(string propertyName)
        {
            var propertyToFind = propertyName;
            var prefix = typeof(TEntity).Name;
            if (propertyToFind.StartsWith(prefix))
            {
                propertyToFind = propertyToFind.Substring(prefix.Length);
            }

            //This is to handle the situation with address where we have properties that start with the type name.
            //We won't find any properties for 1 on address when we are looking for address1 and then rip off the type name.
            var property = TypeDescriptorCache.GetPropertyFor(typeof(TEntity), propertyToFind) ??
                           TypeDescriptorCache.GetPropertyFor(typeof(TEntity), propertyName);

            if (property != null)
            {
                return new SingleProperty(property);
            }

            if (ExtensionProperties.HasExtensionFor(typeof(TEntity)))
            {
                var extensionType = ExtensionProperties.ExtensionFor(typeof(TEntity));
                var extensionProperty = TypeDescriptorCache.GetPropertyFor(extensionType, propertyToFind);
                var extensionAccessorType = typeof(ExtensionPropertyAccessor<>).MakeGenericType(extensionType);
                return (Accessor)Activator.CreateInstance(extensionAccessorType, extensionProperty);
            }

            return null;
        }

        public static bool IsChanged(Type targetType, object newValue, object oldValue)
        {
            if (newValue == null) return oldValue != null;
            if (oldValue == null)
            {
                if (string.Empty.Equals(newValue)) return false;

                return true;
            }

            if (targetType == typeof(TimeZoneInfo))
            {
                return newValue.As<TimeZoneInfo>().Id != oldValue.As<TimeZoneInfo>().Id;
            }

            return !newValue.Equals(oldValue);
        }
    }
}