using System;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
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

        public Registration.Nodes.BehaviorChain TargetChain { get; private set; }

        public string Url { get; private set; }

        public T Get<T>()
        {
            return _services.GetInstance<T>();
        }

        public T Value<T>()
        {
            return (T)Model;
        }

        public FormDef ToFormDef()
        {
            return new FormDef { IsInBound = this.InBound, Id = InBound ? TargetChain.UniqueId.GetHashCode() : Model.GetHashCode(), ModelType = ModelType };
        }

        public class FormElementRequestFactory : IFormElementRequestFactory
        {
            private readonly IChainResolver _chainResolver;
            private readonly IUrlRegistry _urlRegistry;
            private readonly IServiceLocator _serviceLocator;

            // TODO - add caching
            public FormElementRequestFactory(IChainResolver chainResolver, IUrlRegistry urlRegistry,IServiceLocator serviceLocator)
            {
                _chainResolver = chainResolver;
                _urlRegistry = urlRegistry;
                _serviceLocator = serviceLocator;
            }

            public FormElementRequest Create<TController>(Expression<Action<TController>> expression)
            {
                var request = new FormElementRequest(_serviceLocator);
                var chain = _chainResolver.Find(expression);

                request.Model = Activator.CreateInstance(chain.InputType());
                request.Url = _urlRegistry.UrlFor(request.Model);
                return request;
            }

            public FormElementRequest Create(string url)
            {
                return new FormElementRequest(url, _serviceLocator);
            }

            public FormElementRequest Create(object model)
            {
                var request = new FormElementRequest(model, _serviceLocator);

                var chain = _chainResolver.FindUniqueByInputType(model.GetType());

                request.TargetChain = chain;
                request.Url = _urlRegistry.UrlFor(model);

                return request;
            }

        }
    }

    public interface IFormElementRequestFactory
    {
        FormElementRequest Create<TController>(Expression<Action<TController>> expression);

        FormElementRequest Create(string url);

        FormElementRequest Create(object model);
    }

}