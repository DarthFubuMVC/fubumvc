using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using Newtonsoft.Json.Linq;

namespace FubuMVC.Core.Json
{
    public class JObjectValues : IValueSource
    {
        private readonly JObject _jObject;

        public JObjectValues(string json)
            : this(JObject.Parse(json))
        {
        }

        public JObjectValues(JObject jObject)
        {
            _jObject = jObject;
        }

        private bool with<T>(string key, Action<T> action) where T : JToken
        {
            JToken token = null;
            _jObject.TryGetValue(key, out token);

            var target = token as T;
            if (target != null)
            {
                action(target);
                return true;
            }

            return false;
        }

        private TReturn find<T, TReturn>(string key, Func<T, TReturn> creator)
            where T : JToken
            where TReturn : class
        {
            JToken token = null;
            _jObject.TryGetValue(key, out token);

            var specific = token as T;
            return specific == null ? null : creator(specific);
        }

        public bool Has(string key)
        {
            return with<JValue>(key, x => { });
        }

        public object Get(string key)
        {
            return find<JValue, object>(key, v => v.Value<string>());
        }

        public bool HasChild(string key)
        {
            return with<JObject>(key, o => { });
        }

        public IValueSource GetChild(string key)
        {
            return find<JObject, IValueSource>(key, x => new JObjectValues(x));
        }

        public IEnumerable<IValueSource> GetChildren(string key)
        {
            Func<JArray, IEnumerable<IValueSource>> creator = ja => ja.OfType<JObject>().Select(x => new JObjectValues(x)).ToList();
            return find(key, creator) ?? Enumerable.Empty<IValueSource>();
        }

        public void WriteReport(IValueReport report)
        {
            foreach (var pair in _jObject)
            {
                var key = pair.Key;

                pair.IfIs<JValue>(value => report.Value(key, value.Value<string>()));

                pair.IfIs<JObject>(jo =>
                {
                    report.StartChild(key);

                    var child = new JObjectValues(jo);
                    child.WriteReport(report);

                    report.EndChild();
                });

                pair.IfIs<JArray>(ja =>
                {
                    int i = 0;
                    foreach (JObject jo in ja.OfType<JObject>())
                    {
                        report.StartChild(key, i);

                        var child = new JObjectValues(jo);
                        child.WriteReport(report);

                        report.EndChild();
                    }
                });
            }
        }


        public bool Value(string key, Action<BindingValue> callback)
        {
            var found = with<JValue>(key, v => callback(new BindingValue
            {
                RawKey = key,
                RawValue = v.Value<string>(),
                Source = Provenance
            }));

            if (found) return true;

            return with<JObject>(key, v => callback(new BindingValue
            {
                RawKey = key,
                RawValue = v,
                Source = Provenance
            }));
        }

        public string Provenance
        {
            get { return "Json Values"; }
        }
    }
}