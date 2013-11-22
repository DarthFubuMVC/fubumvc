using System;
using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using System.Linq;

namespace FubuMVC.Core.Http
{
    public class HeaderValueSource : IValueSource
    {
        private readonly ICurrentHttpRequest _request;

        public HeaderValueSource(ICurrentHttpRequest request)
        {
            _request = request;
            Provenance = RequestDataSource.Header.ToString();
        }

        public bool Has(string key)
        {
            return _request.HasHeader(key);
        }

        public object Get(string key)
        {
            var value = _request.GetHeader(key);
            return value.Count() == 1 ? (object) value.Single() : value;
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
            _request.AllHeaderKeys().Each(key => {
                report.Value(key, Get(key));
            });
        }

        public bool Value(string key, Action<BindingValue> callback)
        {
            if (!Has(key)) return false;

            callback(new BindingValue
            {
                RawKey = key,
                Source = Provenance,
                RawValue = Get(key)
            });


            return true;
        }

        public string Provenance { get; private set; }
    }
}