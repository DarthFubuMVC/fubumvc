using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

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
            If(p => true).Use((r, prop) => x => BasicConvert(prop.PropertyType, x.Value));
        }

        public ConverterExpression If(Predicate<PropertyInfo> matches)
        {
            return new ConverterExpression(matches, f => _families.Add(f));
        }

        public void Add<T>() where T : IConverterFamily, new()
        {
            _families.Add(new T());
        }

        public static object BasicConvert(Type type, object original)
        {
            if (original == null) return null;

            return type.IsAssignableFrom(original.GetType())
                       ? original
                       : TypeDescriptor.GetConverter(type).ConvertFrom(original);
        }
    }
}