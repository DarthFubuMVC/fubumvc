using System;
using System.Globalization;
using System.Threading;
using FubuCore.Util;

namespace FubuMVC.Core.Localization.Basic
{
    public class LocalizationProviderFactory : ILocalizationProviderFactory
    {
        private readonly ILocalizationStorage _storage;
        private readonly ILocalizationMissingHandler _missingHandler;
        private readonly ILocalizationCache _cache;
        private readonly Cache<CultureInfo, ILocalizationDataProvider> _providers;


        public LocalizationProviderFactory(ILocalizationStorage storage, ILocalizationMissingHandler missingHandler, ILocalizationCache cache)
        {
            _storage = storage;
            _missingHandler = missingHandler;
            _cache = cache;

            _providers = new Cache<CultureInfo, ILocalizationDataProvider>(culture => BuildProvider(culture));
        }

        public void LoadAll(Action<string> tracer)
        {
            _cache.LoadCaches(loader =>
            {
                _storage.LoadAll(tracer, (c, keys) =>
                {
                    loader(c, new ThreadSafeLocaleCache(c, keys));
                });
            });
        }

        public ILocalizationDataProvider BuildProvider(CultureInfo culture)
        {
            return new LocalizationProvider(_cache.CacheFor(culture, () => _storage.Load(culture)), _missingHandler);
        }

        public ILocalizationDataProvider SelectProviderByThread()
        {
            return _providers[Thread.CurrentThread.CurrentUICulture];
        }

        public void ApplyToLocalizationManager()
        {
            LocalizationManager.RegisterProvider(SelectProviderByThread);
        }
    }
}