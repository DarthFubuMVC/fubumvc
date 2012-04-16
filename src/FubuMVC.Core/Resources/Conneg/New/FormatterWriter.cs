using System.Collections.Generic;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Resources.Conneg.New
{
    public class FormatterWriter<T, TFormatter> : IMediaWriter<T> where TFormatter : IFormatter
    {
        private readonly TFormatter _formatter;

        public FormatterWriter(TFormatter formatter)
        {
            _formatter = formatter;
        }

        public void Write(string mimeType, T resource)
        {
            _formatter.Write(resource, mimeType);
        }

        public IEnumerable<string> Mimetypes
        {
            get { return _formatter.MatchingMimetypes; }
        }
    }
}