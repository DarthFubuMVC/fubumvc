using System;
using System.Collections.Generic;
using System.Globalization;

namespace FubuMVC.Core.Localization.Basic
{
    

    public interface ILocalizationStorage
    {
        void WriteMissing(string key, string text, CultureInfo culture);

        void LoadAll(Action<string> tracer, Action<CultureInfo, IEnumerable<LocalString>> callback);

        IEnumerable<LocalString> Load(CultureInfo culture);
    }
}