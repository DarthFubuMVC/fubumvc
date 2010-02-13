using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Models
{
    public class ValueConverterRegistry : IValueConverterRegistry
    {
        private readonly List<IConverterFamily> _families = new List<IConverterFamily>();

        public ValueConverterRegistry(IConverterFamily[] families)
        {
            _families.AddRange(families);

            addPolicies();
        }

        public IEnumerable<IConverterFamily> Families { get { return _families; } }

        // TODO -- harden against not being able to find a Converter
        public ValueConverter FindConverter(PropertyInfo property)
        {
            return _families.Find(x => x.Matches(property)).Build(this, property);
        }

        private void addPolicies()
        {
            Add<ExpandEnvironmentVariablesFamily>();
            Add<MapFromWebPathFamily>();
            Add<ResolveConnectionStringFamily>();

            Add<BooleanFamily>();
            Add<BasicTypeConverter>();
        }

        public void Add<T>() where T : IConverterFamily, new()
        {
            _families.Add(new T());
        }

    }

    public class BasicTypeConverter : IConverterFamily
    {
        public bool Matches(PropertyInfo property)
        {
            try
            {
                return TypeDescriptor.GetConverter(property.PropertyType).CanConvertFrom(typeof (string));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            var propertyType = property.PropertyType;

            return GetValueConverter(propertyType);
        }

        public static ValueConverter GetValueConverter(Type propertyType)
        {
            var converter = TypeDescriptor.GetConverter(propertyType);
            return context => converter.ConvertFrom(context.PropertyValue);
        }
    }
}