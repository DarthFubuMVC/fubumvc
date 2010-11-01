using System;
using System.Collections.Generic;
using System.Reflection;

namespace FubuCore.CommandLine
{
    public class InputParser
    {
        public static readonly string FLAG_PREFIX = "-";
        public static readonly string FLAG_SUFFIX = "Flag";
        private readonly ObjectConverter _converter = new ObjectConverter();

        public object BuildInput(Type inputType, Queue<string> tokens)
        {
            throw new NotImplementedException();
        }

        public ITokenHandler BuildHandler(PropertyInfo property)
        {
            if (!property.Name.EndsWith(FLAG_SUFFIX))
            {
                return new Argument(property, _converter);
            }

            if (property.PropertyType == typeof(bool))
            {
                return new BooleanFlag(property);
            }

            if (property.PropertyType.IsEnum)
            {
                return new EnumerationFlag(property, _converter);
            }

            return new Flag(property, _converter);
        }

        public static string ToFlagName(PropertyInfo property)
        {
            return FLAG_PREFIX + property.Name.TrimEnd(FLAG_SUFFIX.ToCharArray()).ToLower();
        }
    }

    public static class QueueExtensions
    {
        public static bool NextIsFlag(this Queue<string> queue, PropertyInfo property)
        {
            return queue.Peek().ToLower() == InputParser.ToFlagName(property);
        }
    }

    public interface ITokenHandler
    {
        bool Handle(object input, Queue<string> tokens);
    }

    public class BooleanFlag : ITokenHandler
    {
        private readonly PropertyInfo _property;

        public BooleanFlag(PropertyInfo property)
        {
            _property = property;
        }

        public bool Handle(object input, Queue<string> tokens)
        {
            if (tokens.NextIsFlag(_property))
            {
                tokens.Dequeue();
                _property.SetValue(input, true, null);

                return true;
            }

            return false;
        }
    }

    public class EnumerationFlag : ITokenHandler
    {
        private readonly PropertyInfo _property;
        private readonly ObjectConverter _converter;

        public EnumerationFlag(PropertyInfo property, ObjectConverter converter)
        {
            _property = property;
            _converter = converter;
        }

        public bool Handle(object input, Queue<string> tokens)
        {
            throw new NotImplementedException();
        }
    }

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
            throw new NotImplementedException();
        }
    }

    public class Argument : ITokenHandler
    {
        private readonly PropertyInfo _property;
        private readonly ObjectConverter _converter;

        public Argument(PropertyInfo property, ObjectConverter converter)
        {
            _property = property;
            _converter = converter;
        }

        public bool Handle(object input, Queue<string> tokens)
        {
            throw new NotImplementedException();
        }
    }
}