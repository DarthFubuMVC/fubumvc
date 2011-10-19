using System;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.UI.Security;
using FubuMVC.Core.Urls;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;
using FubuMVC.Core.UI.Tags;

namespace FubuMVC.Core.UI.Configuration
{
    public class FormElementRequest
    {
        private readonly IServiceLocator _services;

        public bool InBound
        {
            get
            {
                return !(this.ModelType == typeof(string));
            }
        }

        public Type ModelType
        {
            get { return Model.GetType(); }
        }

        public object Model { get; private set; }

        private FormElementRequest(object model, IServiceLocator services)
        {
            this._services = services;
            this.Model = model;
        }

        private FormElementRequest(IServiceLocator services)
        {
            this._services = services;
        }

        public static FormElementRequest For<TController>(Expression<Action<TController>> expression, IServiceLocator services)
        {
            var request = new FormElementRequest(services);
            var chainResolver = services.GetInstance<IChainResolver>();
            var chain = chainResolver.Find(expression);

            request.Model = Activator.CreateInstance(chain.InputType());
            request.Url = services.GetInstance<IUrlRegistry>().UrlFor(expression);
            return request;
        }

        public static FormElementRequest For(string url, IServiceLocator services)
        {
            return new FormElementRequest(url,services);
        }

        public static FormElementRequest For(object model, IServiceLocator services)
        {
            var request = new FormElementRequest(model, services);

            var chainResolver = services.GetInstance<IChainResolver>();
            var chain = chainResolver.FindUniqueByInputType(model.GetType());
            var urls = services.GetInstance<IUrlRegistry>();
            
            request.TargetChain = chain;
            request.Url = urls.UrlFor(model);

            return request;
        }

        public FubuMVC.Core.Registration.Nodes.BehaviorChain TargetChain { get; private set; }

        public string Url { get; private set; }

        public T Get<T>()
        {
            return _services.GetInstance<T>();
        }

        public virtual ITagGenerator Tags()
        {
            return _services.TagsFor(Model);
        }

        public T Value<T>()
        {
            return (T)Model;
        }

        public FormDef ToFormDef()
        {
            return new FormDef { IsInBound = this.InBound, Id = InBound? TargetChain.UniqueId.GetHashCode() : Model.GetHashCode() , ModelType = ModelType};
        }
    }
}