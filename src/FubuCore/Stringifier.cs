using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using FubuCore.Util;
using Microsoft.Practices.ServiceLocation;

namespace FubuCore
{
    // TODO -- eliminate this stuff.  Go to strictly using the StringConventionRegistry
    public interface IStringifierConfiguration
    {
        void IfIsType<T>(Func<T, string> display);
        void IfIsType<T>(Func<GetStringRequest, T, string> display);
        void IfCanBeCastToType<T>(Func<T, string> display);
        void IfCanBeCastToType<T>(Func<GetStringRequest, T, string> display);
        void IfPropertyMatches(Func<PropertyInfo, bool> matches, Func<object, string> display);
        void IfPropertyMatches(Func<PropertyInfo, bool> matches, Func<GetStringRequest, string> display);
        void IfPropertyMatches<T>(Func<PropertyInfo, bool> matches, Func<T, string> display);
        void IfPropertyMatches<T>(Func<PropertyInfo, bool> matches, Func<GetStringRequest, T, string> display);
        void AddStrategy(StringifierStrategy strategy);
    }
    
    public class Stringifier : IStringifierConfiguration
    {
        // TODO -- make these things have a common interface ?
        private readonly List<StringifierStrategy> _strategies = new List<StringifierStrategy>();
        private readonly List<PropertyOverrideStrategy> _overrides = new List<PropertyOverrideStrategy>();


        private Func<GetStringRequest, string> findConverter(GetStringRequest request)
        {
            if (request.PropertyType.IsNullable())
            {
                if (request.RawValue == null) return r => string.Empty;

                return findConverter(request.GetRequestForNullableType());
            }

            var strategy = _strategies.FirstOrDefault(x => x.Matches(request));
            return strategy == null ? toString : strategy.StringFunction;
        }

        private static string toString(GetStringRequest value)
        {
            return value.RawValue == null ? string.Empty : value.RawValue.ToString();
        }

        public string GetString(GetStringRequest request)
        {
            if (request == null || request.RawValue == null || (request.RawValue as String) == string.Empty) return string.Empty;
            var propertyOverride = _overrides.FirstOrDefault(o => o.Matches(request.Property));

            if (propertyOverride != null)
            {
                return propertyOverride.StringFunction(request);
            }

            return findConverter(request)(request);
        }


        public string GetString(object rawValue)
        {
            if( rawValue == null || (rawValue as String) == string.Empty ) return string.Empty;

            return GetString(new GetStringRequest(null, null, rawValue, null));
        }

        // TODO -- have these things delegate
        public void IfIsType<T>(Func<T, string> display)
        {
            _strategies.Add(new StringifierStrategy
            {
                Matches = request => request.PropertyType == typeof (T),
                StringFunction = o => display((T) o.RawValue)
            });
        }

        public void IfIsType<T>(Func<GetStringRequest, T, string> display)
        {
            _strategies.Add(new StringifierStrategy
            {
                Matches = request => request.PropertyType == typeof(T),
                StringFunction = o => display(o, (T)o.RawValue)
            });
        }

        public void IfCanBeCastToType<T>(Func<T, string> display)
        {
            _strategies.Add(new StringifierStrategy
            {
                Matches = t => t.PropertyType.CanBeCastTo<T>(),
                StringFunction = o => display((T)o.RawValue)
            });
        }

        public void IfCanBeCastToType<T>(Func<GetStringRequest, T, string> display)
        {
            _strategies.Add(new StringifierStrategy
            {
                Matches = t => t.PropertyType.CanBeCastTo<T>(),
                StringFunction = o => display(o, (T)o.RawValue)
            });
        }

        public class PropertyOverrideStrategy
        {
            public Func<PropertyInfo, bool> Matches;
            public Func<GetStringRequest, string> StringFunction;
        }

        public void IfPropertyMatches(Func<PropertyInfo, bool> matches, Func<object, string> display)
        {
            _overrides.Add(new PropertyOverrideStrategy
            {
                Matches = matches, 
                StringFunction = (r) => display(r.RawValue)
            });
        }

        public void IfPropertyMatches(Func<PropertyInfo, bool> matches, Func<GetStringRequest, string> display)
        {
            _overrides.Add(new PropertyOverrideStrategy
            {
                Matches = matches,
                StringFunction = display
            });
        }

        public void IfPropertyMatches<T>(Func<PropertyInfo, bool> matches, Func<T, string> display)
        {
            IfPropertyMatches(p => p.PropertyType.CanBeCastTo<T>() && matches(p), r => display((T)r.RawValue));
        }

        public void IfPropertyMatches<T>(Func<PropertyInfo, bool> matches, Func<GetStringRequest, T, string> display)
        {
            IfPropertyMatches(p => p.PropertyType.CanBeCastTo<T>() && matches(p), r => display(r, (T)r.RawValue));
        }

        public void AddStrategy(StringifierStrategy strategy)
        {
            _strategies.Add(strategy);
        }
    }

    public class StringifierStrategy
    {
        public Func<GetStringRequest, bool> Matches;
        public Func<GetStringRequest, string> StringFunction;
    }

    public class GetStringRequest
    {
        private IServiceLocator _locator;
        public GetStringRequest(){}

        public GetStringRequest(object model, Accessor accessor, object rawValue, IServiceLocator locator)
        {
            _locator = locator;
            Model = model;
            if (accessor != null) Property = accessor.InnerProperty;
            RawValue = rawValue;

            if (Property != null)
            {
                PropertyType = Property.PropertyType;
            }
            else if (RawValue != null)
            {
                PropertyType = RawValue.GetType();
            }

            if (model != null)
            {
                OwnerType = model.GetType();
            }
            else if (accessor != null)
            {
                OwnerType = accessor.OwnerType;
            }
            else if (Property != null)
            {
                OwnerType = Property.DeclaringType;
            }
        }

        public string WithFormat(string format)
        {
            return string.Format(format, RawValue);
        }

        // Yes, I made this internal.  Don't necessarily want it in the public interface,
        // but needs to be "settable"
        internal IServiceLocator Locator { get { return _locator; } set { _locator = value; } }

        public GetStringRequest GetRequestForNullableType()
        {
            return new GetStringRequest()
            {
                _locator = _locator,
                Model = Model,
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

        public Type OwnerType { get; set; }
        private Type _propertyType;
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

        public object Model { get; set; }
        public PropertyInfo Property { get; set; }
        public object RawValue { get; set; }
        public string Format { get; set; }
    }


}