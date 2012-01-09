using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FubuCore;

namespace FubuMVC.OwinHost
{
    public class FormRequestReader : IOwinRequestReader
    {
        private readonly Action<IDictionary<string, string>> _finish;
        private readonly StringBuilder _builder;
        private readonly Encoding _encoding;

        public FormRequestReader(Action<IDictionary<string, string>> finish)
        {
            _finish = finish;
            _builder = new StringBuilder();
            _encoding = Encoding.UTF8; // TODO -- is this a good idea?
        }

        public void Read(byte[] bytes, int offset, int count)
        {
            var text = _encoding.GetString(bytes, offset, count);
            _builder.Append(text);
        }

        public IDictionary<string, string> PostData()
        {
            return Parse(_builder.ToString());
        }

        public static IDictionary<string, string> Parse(string queryString)
        {
            var dictionary = new Dictionary<string, string>();
            var values = (queryString ?? "").Split('&');
            values.Each(x =>
            {
                var parts = x.Split('=');
                dictionary.Add(parts.First().UrlDecode(), parts.Last().UrlDecode());
            });

            return dictionary;

        }

        static string Decode(string value)
        {
            return value.Replace("%3A", ":").Replace("%2F", "/");
        }

        public void Finish()
        {
            _finish(PostData());
        }
    }
}