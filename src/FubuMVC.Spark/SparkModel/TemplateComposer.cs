using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuMVC.Spark.SparkModel
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
        private readonly IParsingRegistrations _parsings;

        public TemplateComposer(TypePool types, IParsingRegistrations parsings)
        {
            _types = types;
            _parsings = parsings;
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
                var bindRequest = new BindRequest
                {
                    Target = t,
                    Parsing = _parsings.ParsingFor(t),
                    Types = _types,
                    TemplateRegistry = templates,
                    Logger = SparkLogger.Default()
                };

                var binders = _binders.Where(x => x.CanBind(bindRequest));
                var policies = _policies.Where(x => x.Matches(t));
                
                binders.Each(binder => binder.Bind(bindRequest));
                policies.Each(policy => policy.Apply(t));
            });
        }
    }    
}