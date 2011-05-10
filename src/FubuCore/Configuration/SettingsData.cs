using System;
using System.Collections.Generic;
using FubuCore.Util;

namespace FubuCore.Configuration
{
    public class SettingsData
    {
        private readonly IDictionary<string, string> _substitutions;
        private readonly Cache<string, string> _values = new Cache<string, string>();

        public SettingsData() : this(SettingCategory.core)
        {
        }

        public SettingsData(SettingCategory category) : this(category, new Dictionary<string, string>())
        {
        }

        public SettingsData(SettingCategory category, IDictionary<string, string> substitutions)
        {
            _substitutions = substitutions;
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
                _values[key] = TemplateParser.Parse(value, _substitutions);
            }
        }

        public SettingsData With(string key, string value)
        {
            _values[key] = value;
            return this;
        }

        public string Provenance
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