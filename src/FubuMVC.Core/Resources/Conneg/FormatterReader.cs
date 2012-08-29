using System;
using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Resources.Conneg
{
    public class FormatterReader<T, TFormatter> : IReader<T>, DescribesItself where TFormatter : IFormatter
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

        public void Describe(Description description)
        {
            var formatter = Description.For(_formatter);
            description.Title = "Reading with " + formatter.Title;
            description.Children["Formatter"] = formatter;
        }
    }
}