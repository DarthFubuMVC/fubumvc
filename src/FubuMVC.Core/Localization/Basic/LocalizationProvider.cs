using System.Globalization;
using System.Reflection;

namespace FubuMVC.Core.Localization.Basic
{
    public class LocalizationProvider : ILocalizationDataProvider
    {
        private readonly ILocaleCache _localeCache;
        private readonly ILocalizationMissingHandler _missingHandler;

        public LocalizationProvider(ILocaleCache localeCache, ILocalizationMissingHandler missingHandler)
        {
            _localeCache = localeCache;
            _missingHandler = missingHandler;
        }

        public CultureInfo Culture
        {
            get { return _localeCache.Culture; }
        }

        public string GetTextForKey(StringToken key)
        {
            var localizationKey = key.ToLocalizationKey();
            return _localeCache
                .Retrieve(localizationKey, () => _missingHandler.FindMissingText(key, _localeCache.Culture));
        }

        public string GetHeader(PropertyInfo property)
        {
            return GetHeader(new PropertyToken(property));
        }

        public string GetHeader(PropertyToken property)
        {
            var localizationKey = new LocalizationKey(property.StringTokenKey);
            return _localeCache
                .Retrieve(localizationKey, () => _missingHandler.FindMissingProperty(property, _localeCache.Culture));
        }
    }
}