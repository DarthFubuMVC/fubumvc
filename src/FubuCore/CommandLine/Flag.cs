using System.Collections.Generic;
using System.Reflection;

namespace FubuCore.CommandLine
{
    public class Flag : ITokenHandler
    {
        private readonly PropertyInfo _property;
        private readonly ObjectConverter _converter;

        public Flag(PropertyInfo property, ObjectConverter converter)
        {
            _property = property;
            _converter = converter;
        }

        public bool Handle(object input, Queue<string> tokens)
        {
            if (tokens.NextIsFlag(_property))
            {
                tokens.Dequeue();
                var rawValue = tokens.Dequeue();
                var value = _converter.FromString(rawValue, _property.PropertyType);

                _property.SetValue(input, value, null);

                return true;
            }


            return false;
        }
    }
}