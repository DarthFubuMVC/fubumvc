using System;
using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Binding.Values;

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
            return _cookies.Has(key);
        }

        public object Get(string key)
        {
            throw new NotImplementedException();
        }

        public bool HasChild(string key)
        {
            throw new NotImplementedException();
        }

        public IValueSource GetChild(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IValueSource> GetChildren(string key)
        {
            throw new NotImplementedException();
        }

        public void WriteReport(IValueReport report)
        {
            throw new NotImplementedException();
        }

        public bool Value(string key, Action<BindingValue> callback)
        {
            throw new NotImplementedException();
        }

        public string Provenance { get; private set; }
    }
}