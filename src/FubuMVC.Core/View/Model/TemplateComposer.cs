using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.View.Model
{
    public interface ITemplateComposer<T> where T : ITemplateFile
    {
        void Compose(ITemplateRegistry<T> templateRegistry);
    }

    public class TemplateComposer<T> : ITemplateComposer<T> where T : ITemplateFile
    {
        private readonly IList<ITemplateBinder<T>> _binders = new List<ITemplateBinder<T>>();
        private readonly IList<ITemplatePolicy<T>> _policies = new List<ITemplatePolicy<T>>();

        private readonly TypePool _types; 
        private readonly IParsingRegistrations<T> _parsings;

        public TemplateComposer(IParsingRegistrations<T> parsings) : this(ViewTypePool.Default(), parsings) {}
        public TemplateComposer(TypePool types, IParsingRegistrations<T> parsings)
        {
            _types = types;
            _parsings = parsings;
        }

        public TemplateComposer<T> AddBinder<TBinder>() where TBinder : ITemplateBinder<T>, new()
        {
            var binder = new TBinder();
            return AddBinder(binder);
        }

        public TemplateComposer<T> AddBinder(ITemplateBinder<T> binder)
        {
            _binders.Add(binder);
            return this;
        }
        public TemplateComposer<T> Apply(ITemplatePolicy<T> policy)
        {
            _policies.Add(policy);
            return this;
        }

        public TemplateComposer<T> Apply<TPolicy>() where TPolicy : ITemplatePolicy<T>, new()
        {
            return Apply(new TPolicy());
        }

        public TemplateComposer<T> Apply<TPolicy>(Action<TPolicy> configure) where TPolicy : ITemplatePolicy<T>, new()
        {
            var policy = new TPolicy();
            configure(policy);
            Apply(policy);
            return this;
        }

        public void Compose(ITemplateRegistry<T> templates)
        {
            templates.Each(t =>
            {
                Compose(t);
            });
        }

        public void Compose(T t)
        {
            var bindRequest = new BindRequest<T>
            {
                Target = t,
                Parsing = _parsings.ParsingFor(t),
                Types = _types,
                Logger = TemplateLogger.Default()
            };

            var binders = _binders.Where(x => x.CanBind(bindRequest));
            var policies = _policies.Where(x => x.Matches(t));

            binders.Each(binder => binder.Bind(bindRequest));
            policies.Each(policy => policy.Apply(t));
        }
    }
}