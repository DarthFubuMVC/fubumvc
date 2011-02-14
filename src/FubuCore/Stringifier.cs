using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using Microsoft.Practices.ServiceLocation;

namespace FubuCore
{
    public class Stringifier
    {
        private readonly List<PropertyOverrideStrategy> _overrides = new List<PropertyOverrideStrategy>();
        private readonly List<StringifierStrategy> _strategies = new List<StringifierStrategy>();


        private Func<GetStringRequest, string> findConverter(GetStringRequest request)
        {
            if (request.PropertyType.IsNullable())
            {
                if (request.RawValue == null) return r => string.Empty;

                return findConverter(request.GetRequestForNullableType());
            }

            StringifierStrategy strategy = _strategies.FirstOrDefault(x => x.Matches(request));
            return strategy == null ? toString : strategy.StringFunction;
        }

        private static string toString(GetStringRequest value)
        {
            return value.RawValue == null ? string.Empty : value.RawValue.ToString();
        }

        public string GetString(GetStringRequest request)
        {
            if (request == null || request.RawValue == null || (request.RawValue as String) == string.Empty)
                return string.Empty;
            PropertyOverrideStrategy propertyOverride = _overrides.FirstOrDefault(o => o.Matches(request.Property));

            if (propertyOverride != null)
            {
                return propertyOverride.StringFunction(request);
            }

            return findConverter(request)(request);
        }


        public string GetString(object rawValue)
        {
            if (rawValue == null || (rawValue as String) == string.Empty) return string.Empty;

            return GetString(new GetStringRequest(null, rawValue, null));
        }


        public void AddStrategy(StringifierStrategy strategy)
        {
            _strategies.Add(strategy);
        }

        #region Nested type: PropertyOverrideStrategy

        public class PropertyOverrideStrategy
        {
            public Func<PropertyInfo, bool> Matches;
            public Func<GetStringRequest, string> StringFunction;
        }

        #endregion
    }

    public class StringifierStrategy
    {
        public Func<GetStringRequest, bool> Matches;
        public Func<GetStringRequest, string> StringFunction;
    }

    public class GetStringRequest
    {
        private IServiceLocator _locator;
        private Type _propertyType;

        public GetStringRequest()
        {
        }


        // TODO -- get some unit tests around this very constructor
        public GetStringRequest(Accessor accessor, object rawValue, IServiceLocator locator)
        {
            _locator = locator;
            if (accessor != null) Property = accessor.InnerProperty;
            RawValue = rawValue;

            setPropertyType();

            setOwnerType(accessor);
        }

        private void setOwnerType(Accessor accessor)
        {
            if (accessor != null)
            {
                OwnerType = accessor.OwnerType;
            }
            else if (Property != null)
            {
                OwnerType = Property.DeclaringType;
            }
        }

        private void setPropertyType()
        {
            if (Property != null)
            {
                PropertyType = Property.PropertyType;
            }
            else if (RawValue != null)
            {
                PropertyType = RawValue.GetType();
            }
        }

        // Yes, I made this internal.  Don't necessarily want it in the public interface,
        // but needs to be "settable"
        internal IServiceLocator Locator { get { return _locator; } set { _locator = value; } }

        public Type OwnerType { get; set; }

        public Type PropertyType
        {
            get
            {
                if (_propertyType == null && Property != null)
                {
                    return Property.PropertyType;
                }

                return _propertyType;
            }
            set { _propertyType = value; }
        }

        public PropertyInfo Property { get; set; }
        public object RawValue { get; set; }
        public string Format { get; set; }

        public string WithFormat(string format)
        {
            return string.Format(format, RawValue);
        }

        public GetStringRequest GetRequestForNullableType()
        {
            return new GetStringRequest
            {
                _locator = _locator,
                Property = Property,
                PropertyType = PropertyType.GetInnerTypeFromNullable(),
                RawValue = RawValue,
                OwnerType = OwnerType
            };
        }

        public T Get<T>()
        {
            return _locator.GetInstance<T>();
        }

        public bool Equals(GetStringRequest other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.OwnerType, OwnerType) && Equals(other.Property, Property) && Equals(other.RawValue, RawValue) && Equals(other.Format, Format);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (GetStringRequest)) return false;
            return Equals((GetStringRequest) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (OwnerType != null ? OwnerType.GetHashCode() : 0);
                result = (result*397) ^ (Property != null ? Property.GetHashCode() : 0);
                result = (result*397) ^ (RawValue != null ? RawValue.GetHashCode() : 0);
                result = (result*397) ^ (Format != null ? Format.GetHashCode() : 0);
                return result;
            }
        }
    }
}