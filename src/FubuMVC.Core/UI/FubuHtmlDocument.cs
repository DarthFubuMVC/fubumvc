using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using HtmlTags;

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

        public string ElementPrefix { get; set; }
        public IServiceLocator ServiceLocator { get; set; }

        public T Get<T>()
        {
            return _services.GetInstance<T>();
        }

        public T GetNew<T>()
        {
            throw new NotImplementedException();
        }

        public void Write(object content)
        {
            Get<IOutputWriter>().WriteHtml(content);
        }

        public IUrlRegistry Urls
        {
            get { return _services.GetInstance<IUrlRegistry>(); }
        }

        public void WriteAssetsToHead()
        {
            Head.Append(this.WriteAssetTags());
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
        public FubuHtmlDocument(IServiceLocator services) : base(services)
        {
        }

        object IFubuPageWithModel.GetModel()
        {
            return Model;
        }

        public T Model { get; set; }

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