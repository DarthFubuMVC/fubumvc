using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace FubuLocalization
{
    public class ThreadSafeLocaleCache : ILocaleCache
    {
        private readonly CultureInfo _culture;
        private readonly IDictionary<LocalizationKey, string> _data;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public ThreadSafeLocaleCache(CultureInfo culture, IDictionary<LocalizationKey, string> data)
        {
            _culture = culture;
            _data = data;
        }

        public CultureInfo Culture { get { return _culture; } }

        public void Append(LocalizationKey key, string value)
        {
            _lock.EnterWriteLock();
            try
            {
                if (_data.ContainsKey(key))
                {
                    _data[key] = value;
                }
                else
                {
                    _data.Add(key, value);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public string Retrieve(LocalizationKey key, Func<string> missing)
        {
            var text = initialRead(key);

            if (text == null)
            {
                text = missing();
                Append(key, text);
            }

            return text;
        }

        private string initialRead(LocalizationKey key)
        {
            _lock.EnterReadLock();
            try
            {
                return !_data.ContainsKey(key) ? null : _data[key];
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}