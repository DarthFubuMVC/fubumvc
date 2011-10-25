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
        private readonly IChunkLoader _chunkLoader;

        public TemplateComposer(TypePool types) : this(types, new ChunkLoader()) { }
        public TemplateComposer(TypePool types, IChunkLoader chunkLoader)
        {
            _types = types;
            _chunkLoader = chunkLoader;
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

                // TODO: Register metadata per template such that we can use it in activation.
                binders.Each(binder => binder.Bind(bindRequest));


                policies.Each(policy => policy.Apply(t));
            });
        }

        private BindRequest createBindRequest(ITemplate template, ITemplateRegistry templateRegistry)
        {
            var chunks = _chunkLoader.Load(template);
            return new BindRequest
            {
                Target = template,
                Types = _types,
                TemplateRegistry = templateRegistry,
                Master = chunks.Master(),
                ViewModelType = chunks.ViewModel(),
                Namespaces = chunks.Namespaces(),
                Logger = SparkLogger.Default()
            };
        }
    }    
}