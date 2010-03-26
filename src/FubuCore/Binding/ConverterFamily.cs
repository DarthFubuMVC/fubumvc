using System;
using System.Reflection;

namespace FubuCore.Binding
{
    public class ConverterExpression
    {
        private readonly Predicate<PropertyInfo> _matches;
        private readonly Action<ConverterFamily> _register;

        public ConverterExpression(Predicate<PropertyInfo> matches, Action<ConverterFamily> register)
        {
            _matches = matches;
            _register = register;
        }

        public void Use(Func<IValueConverterRegistry, PropertyInfo, ValueConverter> builder)
        {
            _register(new ConverterFamily(_matches, builder));
        }
    }

    public class ConverterFamily : IConverterFamily
    {
        private readonly Func<IValueConverterRegistry, PropertyInfo, ValueConverter> _builder;
        private readonly Predicate<PropertyInfo> _matches;

        public ConverterFamily(Predicate<PropertyInfo> matches, Func<IValueConverterRegistry, PropertyInfo, ValueConverter> builder)
        {
            _matches = matches;
            _builder = builder;
        }

        public bool Matches(PropertyInfo property)
        {
            return _matches(property);
        }

        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            return _builder(registry, property);
        }
    }
}