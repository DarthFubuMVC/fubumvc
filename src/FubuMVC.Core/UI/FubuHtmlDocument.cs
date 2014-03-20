using System;
using System.Linq.Expressions;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using HtmlTags;
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

        public string ElementPrefix { get; set; }
        public IServiceLocator ServiceLocator { get; set; }

        public T Get<T>()
        {
            return _services.GetInstance<T>();
        }

        public T GetNew<T>()
        {
            return Get<T>();
        }

        public void Write(object content)
        {
            Get<IOutputWriter>().WriteHtml(content);
        }

        public IUrlRegistry Urls
        {
            get { return _services.GetInstance<IUrlRegistry>(); }
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
        private readonly IFubuRequest _request;
        private T _model;

        public FubuHtmlDocument(IServiceLocator services, IFubuRequest request) : base(services)
        {
            _request = request;
        }

        public T Model
        {
            get
            {
                if (_model == null)
                {
                    _model = _request.Get<T>();
                }
                
                return _model;
            }
            set { _model = value; }
        }

        public void ShowProp(Expression<Func<T, object>> property)
        {
            throw new NotImplementedException();
            //Add(this.Show(property));
        }

        public void Add(Func<FubuHtmlDocument<T>, HtmlTag> func)
        {
            Add(func(this));
        }

        public void Add(Func<FubuHtmlDocument<T>, ITagSource> source)
        {
            source(this).AllTags().Each(Add);
        }

        public object GetModel()
        {
            return Model;
        }
    }
}