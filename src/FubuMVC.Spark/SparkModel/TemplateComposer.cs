using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuMVC.Spark.SparkModel
{
    public interface ITemplateComposer
    {
        ITemplateRegistry Compose(TypePool typePool);
    }

    public class TemplateComposer : ITemplateComposer
    {
        private readonly IList<ITemplateBinder> _binders = new List<ITemplateBinder>();
        private readonly IList<ITemplatePolicy> _policies = new List<ITemplatePolicy>();
        private readonly ITemplateRegistry _templateRegistry;
        
        private readonly IChunkLoader _chunkLoader;

        public TemplateComposer(ITemplateRegistry templateRegistry) : this(templateRegistry, new ChunkLoader()) { }
        public TemplateComposer(ITemplateRegistry templateRegistry, IChunkLoader chunkLoader)
        {
            _templateRegistry = templateRegistry;
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

        public ITemplateRegistry Compose(TypePool typePool)
        {
            _templateRegistry.AllTemplates().Each(t =>
            {
                var bindRequest = createBindRequest(t, typePool);

                var binders = _binders.Where(x => x.CanBind(bindRequest));
                var policies = _policies.Where(x => x.Matches(t));

                binders.Each(binder => binder.Bind(bindRequest));
                policies.Each(policy => policy.Apply(t));
            });

            return _templateRegistry;
        }

        private BindRequest createBindRequest(ITemplate template, TypePool typePool)
        {
            var chunks = _chunkLoader.Load(template);
            return new BindRequest
            {
                Target = template,
                Types = typePool,
                TemplateRegistry = _templateRegistry,
                Master = chunks.Master(),
                ViewModelType = chunks.ViewModel(),
                Namespaces = chunks.Namespaces(),
                Logger = SparkLogger.Default()
            };
        }
    }    
}