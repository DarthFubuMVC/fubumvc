using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Util;

namespace FubuMVC.UI
{
    public class Stringifier
    {
        private readonly Cache<Type, Func<object, string>> _converters = new Cache<Type, Func<object, string>>();

        private readonly List<StringifierStrategy> _strategies = new List<StringifierStrategy>();

        public Stringifier()
        {
            _converters.OnMissing = type =>
            {
                if (type.IsNullable())
                {
                    return instance =>
                    {
                        return instance == null ? string.Empty : _converters[type.GetInnerTypeFromNullable()](instance);
                    };
                }
                var strategy = _strategies.FirstOrDefault(x => x.Matches(type));
                return strategy == null ? toString : strategy.StringFunction;
            };
        }

        private static string toString(object value)
        {
            return value == null ? string.Empty : value.ToString();
        }

        public string GetString(Type type, object rawValue)
        {
            return _converters[type](rawValue);
        }

        public void ForStruct<T>(Func<T, string> display) where T : struct
        {
            _strategies.Add(new StringifierStrategy
            {
                Matches = type => type == typeof (T),
                StringFunction = o => display((T) o)
            });
        }

        public void ForClass<T>(Func<T, string> display) where T : class
        {
            _strategies.Add(new StringifierStrategy
            {
                Matches = type => type == typeof (T),
                StringFunction = o => display(o as T)
            });
        }

        public void ForTypesOf<T>(Func<T, string> display) where T : class
        {
            _strategies.Add(new StringifierStrategy
            {
                Matches = t => t.CanBeCastTo<T>(),
                StringFunction = o => display(o as T)
            });
        }

        public class StringifierStrategy
        {
            public Func<Type, bool> Matches;
            public Func<object, string> StringFunction;
        }
    }
}