using System.Collections.Generic;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Resources.Conneg
{
    public class FormatterReader<T, TFormatter> : IReader<T> where TFormatter : IFormatter
    {
        private readonly TFormatter _formatter;

        public FormatterReader(TFormatter formatter)
        {
            _formatter = formatter;
        }

        public T Read(string mimeType)
        {
            return _formatter.Read<T>();
        }

        public IEnumerable<string> Mimetypes
        {
            get { return _formatter.MatchingMimetypes; }
        }
    }
}