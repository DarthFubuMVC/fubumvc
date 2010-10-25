using System;
using System.Globalization;

namespace FubuLocalization
{
    public interface ILocaleCache
    {
        void Append(LocalizationKey key, string value);
        string Retrieve(LocalizationKey key, Func<string> missing);
        CultureInfo Culture { get; }
    }
}