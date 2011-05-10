using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace FubuCore.Configuration
{
    public static class XmlSettingsParser
    {
        public static SettingsData Parse(string file)
        {
            return Parse(file, new Dictionary<string, string>());
        }

        public static SettingsData Parse(string file, IDictionary<string, string> substitutions)
        {
            var document = new XmlDocument();
            document.Load(file);

            var data = Parse(document.DocumentElement, substitutions);
            data.Provenance = file;

            return data;
        }

        public static SettingsData Parse(XmlElement element, IDictionary<string, string> substitutions)
        {
            var category = (SettingCategory)(element.HasAttribute("category")
                                               ? Enum.Parse(typeof(SettingCategory), element.GetAttribute("category"))
                                               : SettingCategory.core);

            var data = new SettingsData(category, substitutions);

            element.SelectNodes("add").OfType<XmlElement>().Each(elem =>
            {
                var key = elem.GetAttribute("key");
                var value = elem.GetAttribute("value");
                data[key] = value;
            });

            return data;
        }

        public static SettingsData Parse(XmlElement element)
        {
            return Parse(element, new Dictionary<string, string>());
        }


    }
}