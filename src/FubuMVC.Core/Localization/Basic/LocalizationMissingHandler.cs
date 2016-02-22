using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using FubuCore;

namespace FubuMVC.Core.Localization.Basic
{
    public class LocalizationMissingHandler : ILocalizationMissingHandler
    {
        private readonly ILocalizationStorage _storage;
        private readonly CultureInfo _defaultCulture;

        public LocalizationMissingHandler(ILocalizationStorage storage, CultureInfo defaultCulture)
        {
            _storage = storage;
            _defaultCulture = defaultCulture;
        }

        public string FindMissingText(StringToken key, CultureInfo culture)
        {
            var defaultValue = culture.Name + "_" + key.ToLocalizationKey();
            if (key.DefaultValue.IsNotEmpty() && culture.Equals(_defaultCulture))
            {
                defaultValue = key.DefaultValue;
            }

            _storage.WriteMissing(key.ToLocalizationKey().ToString(), defaultValue, culture);

            return defaultValue;
        }

        public string FindMissingProperty(PropertyToken property, CultureInfo culture)
        {
            var defaultValue = culture.Equals(_defaultCulture)
                                   ? property.Header ?? property.DefaultHeaderText(culture) ?? BreakUpCamelCase(property.PropertyName)
                                   : property.DefaultHeaderText(culture) ?? culture.Name + "_" + property.PropertyName;

            _storage.WriteMissing(property.StringTokenKey, defaultValue, culture);

            return defaultValue;
        }

        public static string BreakUpCamelCase(string fieldName)
        {
            var patterns = new[]
                {
                    "([a-z])([A-Z])",
                    "([0-9])([a-zA-Z])",
                    "([a-zA-Z])([0-9])"
                };
            var output = patterns.Aggregate(fieldName,
                (current, pattern) => Regex.Replace(current, pattern, "$1 $2", RegexOptions.IgnorePatternWhitespace));
            return output.Replace('_', ' ');
        }
    }
}