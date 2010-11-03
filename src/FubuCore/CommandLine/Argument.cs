using System.Collections.Generic;
using System.Reflection;

namespace FubuCore.CommandLine
{
    public class Argument : ITokenHandler
    {
        private readonly PropertyInfo _property;
        private readonly ObjectConverter _converter;
        private bool _latched;

        public Argument(PropertyInfo property, ObjectConverter converter)
        {
            _property = property;
            _converter = converter;
        }

        public bool Handle(object input, Queue<string> tokens)
        {
            if (_latched) return false;

            if (tokens.Peek().StartsWith("-")) return false;

            object value = _converter.FromString(tokens.Dequeue(), _property.PropertyType);
            _property.SetValue(input, value, null);

            _latched = true;

            return true;
        }
    }
}