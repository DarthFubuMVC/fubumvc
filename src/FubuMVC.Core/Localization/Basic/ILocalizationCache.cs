using System;
using System.Collections.Generic;
using System.Globalization;

namespace FubuMVC.Core.Localization.Basic
{
    public interface ILocalizationCache
    {
        ILocaleCache CacheFor(CultureInfo culture, Func<IEnumerable<LocalString>> finder);
        void LoadCaches(Action<Action<CultureInfo, ILocaleCache>> loader);
        void Clear();
    }
}