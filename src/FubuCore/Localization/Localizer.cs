using System;
using System.Reflection;

namespace FubuCore.Localization
{
    public static class Localizer
    {
        private static Func<ILocalizationProvider> _providerSource = 
            () => new NulloLocalizationProvider();

        // Localizer just uses an ILocalizationProvider under the covers
        public static Func<ILocalizationProvider> ProviderSource
        {
            set
            {
                _providerSource = value;
            }
        }

        public static ILocalizationProvider Provider
        {
            get
            {
                return _providerSource();
            }
        }

        public static string TextFor(StringToken token)
        {
            return _providerSource().TextFor(token);
        }

        public static string HeaderTextFor(PropertyInfo property)
        {
            return HeaderFor(property).Heading;
        }

        public static string HeaderTextFor(PropertyToken property)
        {
            return HeaderFor(property).Heading;
        }

        public static LocalizedHeader HeaderFor(PropertyInfo property)
        {
            return _providerSource().HeaderFor(property);
        }

        public static LocalizedHeader HeaderFor(PropertyToken property)
        {
            return _providerSource().HeaderFor(property);
        }
    }
}