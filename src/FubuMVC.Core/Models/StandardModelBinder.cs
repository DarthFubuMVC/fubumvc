using System;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Models
{
    public class StandardModelBinder : IModelBinder
    {
        private readonly IValueConverterRegistry _converters;
        private readonly ITypeDescriptorRegistry _typeRegistry;

        public static readonly Func<PropertyInfo, string> DefaultNamingStrategy = p => p.Name;
        public static readonly Func<PropertyInfo, string> DefaultAlternateNamingStrategy = p => p.Name.Replace("_", "-");


        public StandardModelBinder(IValueConverterRegistry converters, ITypeDescriptorRegistry typeRegistry)
        {
            _converters = converters;
            _typeRegistry = typeRegistry;
            NamingStrategy = DefaultNamingStrategy;
            AlternateNamingStrategy = DefaultAlternateNamingStrategy;
        }

        public Func<PropertyInfo, string> NamingStrategy { get; set; }
        public Func<PropertyInfo, string> AlternateNamingStrategy { get; set; }

        public bool Matches(Type type)
        {
            return type.GetConstructors().Count(x => x.GetParameters().Length == 0) == 1;
        }

        public BindResult Bind(Type type, IRequestData data)
        {
            return Bind(type, Activator.CreateInstance(type), data);
        }

        public BindResult Bind(Type type, object instance, IRequestData data)
        {
            var result = new BindResult
            {
                Value = instance
            };

            Populate(result, type, data);

            return result;
        }


        // Only exists for easier testing
        public void Populate(object target, IRequestData data)
        {
            Populate(new BindResult
            {
                Value = target
            }, target.GetType(), data);
        }

        private void Populate(BindResult result, Type type, IRequestData data)
        {
            _typeRegistry.ForEachProperty(type, prop =>
            {
                TryPopulate(data, prop, result);
            });
        }

        private void TryPopulate(IRequestData data, PropertyInfo prop, BindResult result)
        {
            var found = data.Value(NamingStrategy(prop), raw => setPropertyValue(prop, raw, result));

            if (!found)
            {
                data.Value(AlternateNamingStrategy(prop), raw => setPropertyValue(prop, raw, result));
            }
        }

        private void setPropertyValue(PropertyInfo property, object rawValue, BindResult result)
        {
            try
            {
                var value = ConvertValue(property, rawValue);
                property.SetValue(result.Value, value, null);
            }
            catch (Exception e)
            {
                var problem = new ConvertProblem
                {
                    Exception = e,
                    Item = result.Value,
                    Property = property,
                    Value = rawValue
                };

                result.Problems.Add(problem);
            }
        }

        public object ConvertValue(PropertyInfo property, object rawValue)
        {
            return _converters.FindConverter(property)(new RawValue
            {
                Property = property,
                Value = rawValue
            });
        }
    }
}