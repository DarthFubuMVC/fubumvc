using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using FubuCore.Reflection;
using System.Linq;

namespace FubuCore.CommandLine
{
    public abstract class TokenHandlerBase : ITokenHandler
    {
        private readonly PropertyInfo _property;

        protected TokenHandlerBase(PropertyInfo property)
        {
            _property = property;
        }

        public string Description
        {
            get
            {
                var name = _property.Name;
                _property.ForAttribute<DescriptionAttribute>(att => name = att.Description);

                return name;
            }
        }

        public abstract bool Handle(object input, Queue<string> tokens);
        public abstract string ToUsageDescription();
        public virtual bool RequiredForUsage(string usage)
        {
            return false;
        }

        public virtual bool OptionalForUsage(string usage)
        {
            return true;
        }
    }

    public class Argument : TokenHandlerBase
    {
        private readonly ObjectConverter _converter;
        private readonly PropertyInfo _property;
        private bool _latched;

        public Argument(PropertyInfo property, ObjectConverter converter) : base(property)
        {
            _property = property;
            _converter = converter;
        }

        public override bool Handle(object input, Queue<string> tokens)
        {
            if (_latched) return false;

            if (tokens.Peek().StartsWith("-")) return false;

            var value = _converter.FromString(tokens.Dequeue(), _property.PropertyType);
            _property.SetValue(input, value, null);

            _latched = true;

            return true;
        }

        public override string ToUsageDescription()
        {
            if (_property.PropertyType.IsEnum)
            {
                return Enum.GetNames(_property.PropertyType).Join("|");
            }

            return "<{0}>".ToFormat(_property.Name.ToLower());
        }

        public override bool RequiredForUsage(string usage)
        {
            var returnValue = false;
            _property.ForAttribute<RequiredUsageAttribute>(att =>
            {
                returnValue = att.Usages.Contains(usage);
            });

            return returnValue;
        }

        public override bool OptionalForUsage(string usage)
        {
            return false;
        }
    }
}