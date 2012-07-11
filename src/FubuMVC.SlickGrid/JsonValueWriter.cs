using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.SlickGrid
{
    public interface IMakeMyOwnJsonValue
    {
        object ToJsonValue();
    }

    // Has to deal with Date's, DateTime's
    public static class JsonValueWriter
    {
        private static readonly Cache<Type, Func<object, object>> _policies = new Cache<Type, Func<object, object>>();

        static JsonValueWriter()
        {
            Clear();
        }

        public static void Clear()
        {
            _policies.ClearAll();

            RegisterPolicy<DateTime>(date => date.ToString());
        }

        public static void RegisterPolicy<T>(Func<T, object> converter)
        {
            Func<object, object> policy = o => converter((T) o);
            _policies[typeof (T)] = policy;
        }

        public static object ConvertToJson(object value)
        {
            if (value == null) return null;

            if (value is Type)
            {
                var type = (Type)value;
                return new Dictionary<string, object>{
                    {"Name", type.Name},
                    {"FullName", type.FullName},
                    {"Namespace", type.Namespace},
                    {"Assembly", type.Assembly.FullName}
                };
            }

            if (_policies.Has(value.GetType()))
            {
                return _policies[value.GetType()](value);
            }

            if (value is IMakeMyOwnJsonValue)
            {
                return value.As<IMakeMyOwnJsonValue>().ToJsonValue();
            }

            return value;
        }
    }
}