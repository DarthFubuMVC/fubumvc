using System;
using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Models
{
    public interface ITypeDescriptorRegistry
    {
        IDictionary<string, PropertyInfo> GetPropertiesFor<TYPE>();
        IDictionary<string, PropertyInfo> GetPropertiesFor(Type itemType);
        void ForEachProperty(Type itemType, Action<PropertyInfo> action);
        void ClearAll();
    }

    public class TypeDescriptorRegistry : ITypeDescriptorRegistry
    {
        private readonly Cache<Type, IDictionary<string, PropertyInfo>> _cache;

        public TypeDescriptorRegistry()
        {
            _cache = new Cache<Type, IDictionary<string, PropertyInfo>>(type =>
            {
                var dict = new Dictionary<string, PropertyInfo>();

                foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    dict.Add(propertyInfo.Name, propertyInfo);
                }

                return dict;
            });
        }

        public IDictionary<string, PropertyInfo> GetPropertiesFor<TYPE>()
        {
            return GetPropertiesFor(typeof (TYPE));
        }

        public IDictionary<string, PropertyInfo> GetPropertiesFor(Type itemType)
        {
            return _cache[itemType];
        }

        public void ForEachProperty(Type itemType, Action<PropertyInfo> action)
        {
            _cache[itemType].Values.Each(action);
        }

        public void ClearAll()
        {
            _cache.ClearAll();
        }
    }
}