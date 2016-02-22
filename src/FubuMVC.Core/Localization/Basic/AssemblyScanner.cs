using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FubuCore;

namespace FubuMVC.Core.Localization.Basic
{
    public interface ILocalizedProperties
    {
        IEnumerable<PropertyInfo> FindProperties();
        IEnumerable<StringToken> FindTokens();
    }

    public class AssemblyScanner
    {
        private readonly ILocalizationStorage _storage;
        private readonly CultureInfo _culture;
        private readonly Lazy<ILocalizationDataProvider> _provider;

        public AssemblyScanner(ILocalizationStorage storage, CultureInfo culture)
        {
            DefaultCulture = new CultureInfo("en-US");

            _storage = storage;
            _culture = culture;

            _provider = new Lazy<ILocalizationDataProvider>(() =>
            {
                var missingHandler = new LocalizationMissingHandler(_storage, DefaultCulture);
                var factory = new LocalizationProviderFactory(_storage, missingHandler,
                                                new LocalizationCache());

                return factory.BuildProvider(_culture);
            });
        }

        public CultureInfo DefaultCulture { get; set; }

        public void ScanAssembly(Assembly assembly)
        {
            assembly
                .GetExportedTypes()
                .Where(x => x.CanBeCastTo<StringToken>())
                .Each(ScanStringTokenType);

            assembly
                .GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<ILocalizedProperties>())
                .Each(type => ScanProperties(type.Create<ILocalizedProperties>()));
        }

        public void ScanStringTokenType(Type type)
        {
            type
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(field => field.FieldType.CanBeCastTo<StringToken>())
                .Each(field =>
                {
                    var token = field.GetValue(null).As<StringToken>();
                    _provider.Value.GetTextForKey(token);
                });

        }

        public void ScanProperties(ILocalizedProperties properties)
        {
            properties.FindProperties().Each(prop => _provider.Value.GetHeader(prop));

            properties.FindTokens().Each(x => _provider.Value.GetTextForKey(x));
        }


    }
}