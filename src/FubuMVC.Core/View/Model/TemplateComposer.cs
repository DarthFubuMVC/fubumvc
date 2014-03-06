using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
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

        private readonly TypePool _types; 

        public TemplateComposer() : this(ViewTypePool.Default()) {}
        public TemplateComposer(TypePool types)
        {
            _types = types;
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


        [MarkedForTermination("We can kill this")]
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
                Types = _types,
                Logger = TemplateLogger.Default()
            };

            var binders = _binders.Where(x => x.CanBind(bindRequest));

            binders.Each(binder => binder.Bind(bindRequest));
        }
    }
}