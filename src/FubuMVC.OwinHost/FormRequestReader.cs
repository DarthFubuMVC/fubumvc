using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            // TODO: this is wrong in many, many ways
            return (queryString ?? "").Split("&".ToCharArray())
                .Select(item => item.Split("=".ToCharArray(), 2))
                .Where(item => item.Length == 2)
                .GroupBy(item => item[0], item => Decode(item[1]), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => string.Join(",", g.ToArray()), StringComparer.OrdinalIgnoreCase);

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