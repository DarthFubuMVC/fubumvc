using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FubuCore.Util;

namespace FubuMVC.Core.Caching
{
    public class SimpleVaryBy : IVaryBy
    {
        private readonly Cache<string, string> _values = new Cache<string, string>();

        public SimpleVaryBy With(string key, string value)
        {
            this[key] = value;
            return this;
        }

        [IndexerName("Items")]
        public string this[string key]
        {
            get
            {
                return _values[key];
            }
            set
            {
                _values[key] = value;
            }
        }

        public IDictionary<string, string> Values()
        {
            return _values.ToDictionary();
        }
    }
}