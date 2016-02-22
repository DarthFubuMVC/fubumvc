using System;
using System.Globalization;

namespace FubuMVC.Core.Localization.Basic
{
    public interface ILocalizationProviderFactory
    {
        void LoadAll(Action<string> tracer);
        ILocalizationDataProvider BuildProvider(CultureInfo culture);
        void ApplyToLocalizationManager();
    }
}