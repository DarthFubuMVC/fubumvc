using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace FubuCore.Configuration
{
    public class XmlSettingsData : ISettingsData
    {
        private readonly SettingCategory _category;
        private readonly IDictionary<string, string> _values = new Dictionary<string, string>();

        public XmlSettingsData(XmlElement element)
        {
            _category = (SettingCategory) (element.HasAttribute("category")
                                               ? Enum.Parse(typeof (SettingCategory), element.GetAttribute("category"))
                                               : SettingCategory.core);

            element.SelectNodes("add").OfType<XmlElement>().Each(elem =>
            {
                _values.Add(elem.GetAttribute("key"), elem.GetAttribute("value"));
            });
        }

        public string Description { get; set; }

        public SettingCategory Category
        {
            get { return _category; }
        }

        public bool Has(string key)
        {
            return _values.ContainsKey(key);
        }

        public string Get(string key)
        {
            return _values[key];
        }

        public IEnumerable<string> AllKeys
        {
            get { return _values.Keys; }
        }
    }
}