using System.Collections.Specialized;
using FubuCore;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http
{
    public class ConnegQuerystring
    {
        public ConnegQuerystring(string key, string value, string mimetype)
        {
            Key = key;
            Value = value;
            Mimetype = mimetype;
        }

        public ConnegQuerystring(string key, string value, MimeType mimetype)
        {
            Key = key;
            Value = value;
            Mimetype = mimetype.Value;
        }

        public string Key { get; private set; }
        public string Value { get; private set; }
        public string Mimetype { get; private set; }

        public string Determine(NameValueCollection querystring)
        {
            var value = querystring[Key];


            return value != null && value.EqualsIgnoreCase(Value)
                ? Mimetype
                : null;
        }
    }
}