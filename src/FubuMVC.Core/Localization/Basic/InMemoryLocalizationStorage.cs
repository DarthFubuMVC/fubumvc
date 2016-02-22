using System;
using System.Collections.Generic;
using System.Globalization;
using FubuCore.Util;

namespace FubuMVC.Core.Localization.Basic
{
    public class InMemoryLocalizationStorage : ILocalizationStorage
    {
        private readonly Cache<CultureInfo, IList<LocalString>> _cache = new Cache<CultureInfo, IList<LocalString>>(x => new List<LocalString>());
        private readonly Cache<CultureInfo, IList<LocalString>> _missing = new Cache<CultureInfo, IList<LocalString>>(x => new List<LocalString>());
        

        public void Add(CultureInfo culture, string key, string text)
        {
            _cache[culture].Add(new LocalString(key, text));
        }

        public void Add(CultureInfo culture, string valuesText)
        {
            _cache[culture].AddRange(LocalString.ReadAllFrom(valuesText));
        }

        public void WriteMissing(string key, string text, CultureInfo culture)
        {
            _missing[culture].Add(new LocalString(key, text));
        }

        public void LoadAll(Action<string> tracer, Action<CultureInfo, IEnumerable<LocalString>> callback)
        {
            _cache.Each(callback);
        }

        public IEnumerable<LocalString> Load(CultureInfo culture)
        {
            return _cache[culture];
        }

        public IEnumerable<LocalString> RecordedMissingKeysFor(CultureInfo culture)
        {
            return _missing[culture];
        }
    }
}