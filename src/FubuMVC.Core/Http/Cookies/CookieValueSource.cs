using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using System.Linq;

namespace FubuMVC.Core.Http.Cookies
{
    public class CookieValueSource : IValueSource
    {
        private readonly ICookies _cookies;

        public CookieValueSource(ICookies cookies)
        {
            _cookies = cookies;
            Provenance = RequestDataSource.Cookie.ToString();
        }

        public bool Has(string key)
        {
            Cookie cookie = _cookies.Get(key);
            return cookie != null && cookie.Value.IsNotEmpty();
        }

        public object Get(string key)
        {
            Cookie cookie = _cookies.Get(key);
            if (cookie == null) return null;

            return cookie.Value ?? cookie.GetValue(key);
        }

        public bool HasChild(string key)
        {
            return false;
        }

        public IValueSource GetChild(string key)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<IValueSource> GetChildren(string key)
        {
            return Enumerable.Empty<IValueSource>();
        }

        public void WriteReport(IValueReport report)
        {
            _cookies.All.Where(x => x.Value.IsNotEmpty()).Each(x => {
                report.Value(x.States.First().Name, x.Value);
            });
        }

        public bool Value(string key, Action<BindingValue> callback)
        {
            var value = Get(key);

            if (value == null) return false;

            callback(new BindingValue
            {
                RawKey = key,
                RawValue = value,
                Source = Provenance
            });

            return true;
        }

        public string Provenance { get; private set; }
    }
}