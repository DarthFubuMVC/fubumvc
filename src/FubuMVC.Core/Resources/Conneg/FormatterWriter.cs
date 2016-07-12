using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuCore.Descriptions;
using FubuMVC.Core.Runtime;
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

        public Task Write(string mimeType, IFubuRequestContext context, T resource)
        {
            return _formatter.Write(context, resource, mimeType);
        }

        public IEnumerable<string> Mimetypes => _formatter.MatchingMimetypes;

        public void Describe(Description description)
        {
            var formatterDescription = Description.For(_formatter);
            description.ShortDescription = null;
            description.Title = "Write with formatter '{0}'".ToFormat(formatterDescription.Title);
            description.Children["Formatter"] = formatterDescription;
        }

        public IFormatter Formatter => _formatter;
    }
}