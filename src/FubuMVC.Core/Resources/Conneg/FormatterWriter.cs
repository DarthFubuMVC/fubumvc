using System;
using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core.Runtime.Formatters;
using FubuCore;

namespace FubuMVC.Core.Resources.Conneg
{
    public class FormatterWriter<T, TFormatter> : IMediaWriter<T>, DescribesItself where TFormatter : IFormatter
    {
        private readonly TFormatter _formatter;

        public FormatterWriter(TFormatter formatter)
        {
            _formatter = formatter;
        }

        public void Write(string mimeType, IFubuRequestContext context, T resource)
        {
            _formatter.Write(resource, mimeType);
        }

        public IEnumerable<string> Mimetypes
        {
            get { return _formatter.MatchingMimetypes; }
        }

        public void Describe(Description description)
        {
            var formatterDescription = Description.For(_formatter);
            description.Title = "Write with formatter '{0}'".ToFormat(formatterDescription.Title);
            description.Children["Formatter"] = formatterDescription;
        }
    }
}