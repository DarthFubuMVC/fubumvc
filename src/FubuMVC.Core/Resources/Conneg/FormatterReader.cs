using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Descriptions;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Resources.Conneg
{

    public class FormatterReader<T> : IReader<T>, DescribesItself
    {
        private readonly IFormatter _formatter;

        public FormatterReader(IFormatter formatter)
        {
            _formatter = formatter;
        }

        public T Read(string mimeType, IFubuRequestContext context)
        {
            var model = _formatter.Read<T>(context);

            context.Services.GetInstance<IBindingContext>().BindProperties(model);

            return model;
        }

        public IEnumerable<string> Mimetypes
        {
            get { return _formatter.MatchingMimetypes; }
        }

        public void Describe(Description description)
        {
            var formatter = Description.For(_formatter);
            description.ShortDescription = null;
            description.Title = "Reading with " + formatter.Title;
            description.Children["Formatter"] = formatter;
        }

        public IFormatter Formatter
        {
            get { return _formatter; }
        }

    }
}