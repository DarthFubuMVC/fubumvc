using System;
using System.Globalization;

namespace FubuMVC.Core.Localization
{
    // SAMPLE: ILocaleCache
    public interface ILocaleCache
    {
        void Append(LocalizationKey key, string value);
        string Retrieve(LocalizationKey key, Func<string> missing);
        CultureInfo Culture { get; }
    }
    // ENDSAMPLE
}