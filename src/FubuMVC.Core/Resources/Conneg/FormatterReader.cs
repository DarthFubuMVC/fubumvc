using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Descriptions;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Resources.Conneg
{
    [MarkedForTermination]
    public class FormatterReader<T, TFormatter> : IReader<T>, DescribesItself where TFormatter : IFormatter
    {
        private readonly TFormatter _formatter;
        private readonly IObjectResolver _objectResolver;
        private readonly IBindingContext _bindingContext;

        public FormatterReader(TFormatter formatter, IObjectResolver objectResolver, IBindingContext bindingContext)
        {
            _formatter = formatter;
            _objectResolver = objectResolver;
            _bindingContext = bindingContext;
        }

        public T Read(string mimeType, IFubuRequestContext context)
        {
            var model = _formatter.Read<T>(context);
            _objectResolver.BindProperties(model, _bindingContext);
            return model;
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

        public Type ModelType
        {
            get
            {
                return typeof(T);
            }
        }
    }

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
            description.Title = "Reading with " + formatter.Title;
            description.Children["Formatter"] = formatter;
        }

        public IFormatter Formatter
        {
            get { return _formatter; }
        }
    }
}