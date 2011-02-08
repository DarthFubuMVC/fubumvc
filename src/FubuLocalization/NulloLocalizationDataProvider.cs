using System.Globalization;
using System.Reflection;
using System.Threading;

namespace FubuLocalization
{
    public class NulloLocalizationDataProvider : ILocalizationDataProvider
    {
        public NulloLocalizationDataProvider(CultureInfo culture)
        {
            Culture = culture;
        }

        public NulloLocalizationDataProvider()
            : this(Thread.CurrentThread.CurrentUICulture)
        {
        }

        public CultureInfo Culture { get; set; }

        public string GetTextForKey(StringToken key)
        {
            return key.DefaultValue ?? Culture.Name + "_" + key.Key;
        }

        public string GetTextForListValue(string listName, string key)
        {
            return Culture.Name + "_" + listName + "-" + key;
        }

        public string GetDefaultText(PropertyToken property)
        {
            return Culture.Name + "_" + property.PropertyName;
        }

        public string GetHeader(PropertyInfo property)
        {
            return Culture.Name + "_" + property.Name;
        }

        public string GetHeader(PropertyToken property)
        {
            return Culture.Name + "_" + property.PropertyName;
        }
    }
}