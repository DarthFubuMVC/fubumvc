using System;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.Serialization;
using System.Threading;

namespace FubuCore.Util
{
    public class Lazy<T>
    {
        private readonly Func<T> _source;
        private readonly object _locker = new object();
        private T _value;

        public Lazy(Func<T> source)
        {
            _source = source;
        }


        public T Value
        {
            get
            {
                if (_value == null)
                {
                    lock (_locker)
                    {
                        if (_value == null)
                        {
                            _value = _source();
                        }
                    }
                }

                return _value;
            }
        }
    }

}