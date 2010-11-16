using System.Globalization;
using System.Reflection;

namespace FubuLocalization
{
    public abstract class BasicLocalizationProvider : ILocalizationDataProvider
    {
        private readonly ILocaleCache _localeCache;
        private readonly ILocalizationMissingHandler _missingHandler;

        protected BasicLocalizationProvider(ILocaleCache localeCache, ILocalizationMissingHandler missingHandler)
        {
            _localeCache = localeCache;
            _missingHandler = missingHandler;
        }

        public CultureInfo Culture { get { return _localeCache.Culture; } }

        public string GetTextForKey(StringToken key)
        {
            LocalizationKey localizationKey = key.ToLocalizationKey();
            return _localeCache
                        .Retrieve(localizationKey, () => _missingHandler.FindMissingText(key, _localeCache.Culture));
        }

        public string GetHeader(PropertyInfo property)
        {
            return GetHeader(new PropertyToken(property));
        }

        public string GetHeader(PropertyToken property)
        {
            LocalizationKey localizationKey = property.ToLocalizationKey();
            return _localeCache
                        .Retrieve(localizationKey, () => _missingHandler.FindMissingProperty(property, _localeCache.Culture));
        }

        public abstract string GetDefaultText(PropertyToken property);
    }
}