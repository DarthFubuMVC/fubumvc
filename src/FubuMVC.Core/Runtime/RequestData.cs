using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Runtime
{
    public class RequestData : IRequestData
    {
        protected AggregateDictionary _dictionary;

        public RequestData(AggregateDictionary dictionary)
        {
            _dictionary = dictionary;
        }

        public void Value(string key, Action<object> callback)
        {
            _dictionary.Value(key, (s, o) =>
            {
                record(key, s, o);
                callback(o);
            });
        }

        public static RequestData ForDictionary(IDictionary<string, object> dictionary)
        {
            AggregateDictionary dict = new AggregateDictionary().AddDictionary(dictionary);
            return new RequestData(dict);
        }

        protected virtual void record(string key, RequestDataSource source, object @object)
        {
        }
    }
}