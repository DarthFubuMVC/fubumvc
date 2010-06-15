using System;
using System.Collections.Generic;
using System.Data;
using FubuCore.Util;

namespace FubuCore.Binding
{
    public class DataReaderRequestData : IRequestData
    {
        private readonly Cache<string, string> _aliases = new Cache<string, string>(key => key);
        private readonly Dictionary<string, string> _columns;
        private readonly IDataReader _reader;

        public DataReaderRequestData(IDataReader reader, Cache<string, string> aliases) : this(reader)
        {
            aliases.OnMissing = key => key;
            _aliases = aliases;
        }

        public DataReaderRequestData(IDataReader reader)
        {
            _reader = reader;


            _columns = new Dictionary<string, string>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                _columns.Add(reader.GetName(i), null);
            }
        }

        public object Value(string key)
        {
            return _reader[_aliases[key]];
        }

        public bool Value(string key, Action<object> callback)
        {
            string column = _aliases[key];
            if (_columns.ContainsKey(column))
            {
                object rawValue = _reader[column];
                if (rawValue == DBNull.Value)
                {
                    callback(null);
                }
                else
                {
                    callback(rawValue.ToString());
                }


                return true;
            }

            return false;
        }

        public void SetAlias(string name, string alias)
        {
            _aliases[name] = alias;
        }
    }
}