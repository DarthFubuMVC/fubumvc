using System;
using System.Linq.Expressions;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;

namespace FubuMVC.Core.UI
{
    public class FubuHtmlDocument : HtmlDocument, IFubuPage
    {
        private readonly IServiceLocator _services;

        public FubuHtmlDocument(IServiceLocator services)
        {
            _services = services;
            ElementPrefix = string.Empty;
        }

        public string ElementPrefix { get; set;}
        public IServiceLocator ServiceLocator { get; set;}

        public T Get<T>()
        {
            return _services.GetInstance<T>();
        }

        public T GetNew<T>()
        {
            throw new NotImplementedException();
        }

        public IUrlRegistry Urls
        {
            get { return _services.GetInstance<IUrlRegistry>(); }
        }

        public void WriteScriptsToBody()
        {
            Body.Append(this.WriteScriptTags());
        }

        public void Add(Func<FubuHtmlDocument, HtmlTag> func)
        {
            Add(func(this));
        }

        public void Add(Func<FubuHtmlDocument, ITagSource> source)
        {
            source(this).AllTags().Each(Add);
        }
    }

    public class FubuHtmlDocument<T> : FubuHtmlDocument, IFubuPage<T> where T : class
    {
        private T _model;

        public FubuHtmlDocument(IServiceLocator services) : base(services)
        {
        }

        object IFubuPageWithModel.GetModel()
        {
            return Model;
        }

        public T Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public void ShowProp(Expression<Func<T, object>> property)
        {
            Add(this.Show(property));
        }

        public void Add(Func<FubuHtmlDocument<T>, HtmlTag> func)
        {
            Add(func(this));
        }

        public void Add(Func<FubuHtmlDocument<T>, ITagSource> source)
        {
            source(this).AllTags().Each(Add);
        }
    } 
}