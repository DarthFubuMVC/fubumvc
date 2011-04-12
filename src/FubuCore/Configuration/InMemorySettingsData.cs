using System.Collections.Generic;
using FubuCore.Util;

namespace FubuCore.Configuration
{
    public class InMemorySettingsData : ISettingsData
    {
        private readonly Cache<string, string> _values = new Cache<string, string>();

        public InMemorySettingsData() : this(SettingCategory.core)
        {
        }

        public InMemorySettingsData(SettingCategory category)
        {
            Category = category;
        }

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

        public InMemorySettingsData With(string key, string value)
        {
            _values[key] = value;
            return this;
        }

        public string Description
        {
            get; set;
        }

        public SettingCategory Category
        {
            get; set;
        }

        public bool Has(string key)
        {
            return _values.Has(key);
        }

        public string Get(string key)
        {
            return _values[key];
        }

        public IEnumerable<string> AllKeys
        {
            get { return _values.GetAllKeys(); }
        }
    }
}