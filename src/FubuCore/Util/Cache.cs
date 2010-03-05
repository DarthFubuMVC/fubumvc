using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FubuCore.Util
{
    public class Cache<TKey, TValue> : IEnumerable<TValue> where TValue : class
    {
        private readonly object _locker = new object();
        private readonly IDictionary<TKey, TValue> _values;

        private Func<TValue, TKey> _getKey = delegate { throw new NotImplementedException(); };

        private Func<TKey, TValue> _onMissing = delegate(TKey key)
        {
            string message = string.Format("Key '{0}' could not be found", key);
            throw new KeyNotFoundException(message);
        };

        public Cache()
            : this(new Dictionary<TKey, TValue>())
        {
        }

        public Cache(Func<TKey, TValue> onMissing)
            : this(new Dictionary<TKey, TValue>(), onMissing)
        {
        }

        public Cache(IDictionary<TKey, TValue> dictionary, Func<TKey, TValue> onMissing)
            : this(dictionary)
        {
            _onMissing = onMissing;
        }

        public Cache(IDictionary<TKey, TValue> dictionary)
        {
            _values = dictionary;
        }

        public Func<TKey, TValue> OnMissing { set { _onMissing = value; } }

        public Func<TValue, TKey> GetKey { get { return _getKey; } set { _getKey = value; } }

        public int Count { get { return _values.Count; } }

        public TValue First
        {
            get
            {
                foreach (var pair in _values)
                {
                    return pair.Value;
                }

                return null;
            }
        }

        public TValue this[TKey key] { get { return Retrieve(key); } set { Store(key, value); } }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TValue>) this).GetEnumerator();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _values.Values.GetEnumerator();
        }

        public void Store(TKey key, TValue value)
        {
            if (_values.ContainsKey(key))
            {
                _values[key] = value;
            }
            else
            {
                _values.Add(key, value);
            }
        }

        public void Fill(TKey key, TValue value)
        {
            if (_values.ContainsKey(key))
            {
                return;
            }

            _values.Add(key, value);
        }

        public TValue Retrieve(TKey key)
        {
            if (!_values.ContainsKey(key))
            {
                lock (_locker)
                {
                    if (!_values.ContainsKey(key))
                    {
                        TValue value = _onMissing(key);
                        _values.Add(key, value);
                    }
                }
            }

            return _values[key];
        }

        public bool TryRetrieve(TKey key, out TValue value)
        {
            value = default(TValue);

            if (_values.ContainsKey(key))
            {
                value = _values[key];
                return true;
            }

            return false;
        }

        public void Each(Action<TValue> action)
        {
            foreach (var pair in _values)
            {
                action(pair.Value);
            }
        }

        public void Each(Action<TKey, TValue> action)
        {
            foreach (var pair in _values)
            {
                action(pair.Key, pair.Value);
            }
        }

        public bool Has(TKey key)
        {
            return _values.ContainsKey(key);
        }

        public bool Exists(Predicate<TValue> predicate)
        {
            bool returnValue = false;

            Each(delegate(TValue value) { returnValue |= predicate(value); });

            return returnValue;
        }

        public TValue Find(Predicate<TValue> predicate)
        {
            foreach (var pair in _values)
            {
                if (predicate(pair.Value))
                {
                    return pair.Value;
                }
            }

            return null;
        }

        public TKey[] GetAllKeys()
        {
            return _values.Keys.ToArray();
        }

        public TValue[] GetAll()
        {
            return _values.Values.ToArray();
        }

        public void Remove(TKey key)
        {
            if (_values.ContainsKey(key))
            {
                _values.Remove(key);
            }
        }

        public void ClearAll()
        {
            _values.Clear();
        }

        public bool WithValue(TKey key, Action<TValue> callback)
        {
            if (Has(key))
            {
                callback(this[key]);
                return true;
            }

            return false;
        }
    }
}