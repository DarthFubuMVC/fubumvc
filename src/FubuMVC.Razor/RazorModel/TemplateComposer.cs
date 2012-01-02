using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Razor.Registration;

namespace FubuMVC.Razor.RazorModel
{
    public interface ITemplateComposer
    {
        void Compose(ITemplateRegistry templateRegistry);
    }

    public class TemplateComposer : ITemplateComposer
    {
        private readonly IList<ITemplateBinder> _binders = new List<ITemplateBinder>();
        private readonly IList<ITemplatePolicy> _policies = new List<ITemplatePolicy>();
        private readonly TypePool _types;
        private readonly IViewLoaderLocator _viewLoaderLocator;

        public TemplateComposer(TypePool types) : this(types, new ViewLoaderLocator()) { }
        public TemplateComposer(TypePool types, IViewLoaderLocator viewLoaderLocator)
        {
            _types = types;
            _viewLoaderLocator = viewLoaderLocator;
        }

        public TemplateComposer AddBinder<T>() where T : ITemplateBinder, new()
        {
            var binder = new T();
            return AddBinder(binder);
        }

        public TemplateComposer AddBinder(ITemplateBinder binder)
        {
            _binders.Add(binder);
            return this;
        }
        public TemplateComposer Apply(ITemplatePolicy policy)
        {
            _policies.Add(policy);
            return this;
        }

        public TemplateComposer Apply<T>() where T : ITemplatePolicy, new()
        {
            return Apply(new T());
        }

        public TemplateComposer Apply<T>(Action<T> configure) where T : ITemplatePolicy, new()
        {
            var policy = new T();
            configure(policy);
            Apply(policy);
            return this;
        }

        public void Compose(ITemplateRegistry templates)
        {
            templates.AllTemplates().Each(t =>
            {
                var bindRequest = createBindRequest(t, templates);

                var binders = _binders.Where(x => x.CanBind(bindRequest));
                var policies = _policies.Where(x => x.Matches(t));

                binders.Each(binder => binder.Bind(bindRequest));
                policies.Each(policy => policy.Apply(t));
            });
        }

        private BindRequest createBindRequest(ITemplate template, ITemplateRegistry templateRegistry)
        {
            var viewFile = _viewLoaderLocator.Locate(template);
            var parser = new ViewParser(viewFile);
            var chunks = parser.Parse();
            return new BindRequest
            {
                Target = template,
                Types = _types,
                TemplateRegistry = templateRegistry,
                Master = chunks.Master(),
                ViewModelType = chunks.ViewModel(),
                Namespaces = chunks.Namespaces(),
                Logger = RazorLogger.Default(),
                ViewFile = viewFile
            };
        }
    }    
}