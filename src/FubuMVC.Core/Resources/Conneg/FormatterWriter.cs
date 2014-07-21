using System;
using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core.Runtime.Formatters;
using FubuCore;

namespace FubuMVC.Core.Resources.Conneg
{
    public class FormatterWriter<T> : IMediaWriter<T>, DescribesItself 
    {
        private readonly IFormatter _formatter;

        public FormatterWriter(IFormatter formatter)
        {
            _formatter = formatter;
        }

        public void Write(string mimeType, IFubuRequestContext context, T resource)
        {
            _formatter.Write(context, resource, mimeType);
        }

        public IEnumerable<string> Mimetypes
        {
            get { return _formatter.MatchingMimetypes; }
        }

        public void Describe(Description description)
        {
            var formatterDescription = Description.For(_formatter);
            description.ShortDescription = null;
            description.Title = "Write with formatter '{0}'".ToFormat(formatterDescription.Title);
            description.Children["Formatter"] = formatterDescription;
        }

        public IFormatter Formatter
        {
            get { return _formatter; }
        }
    }
}