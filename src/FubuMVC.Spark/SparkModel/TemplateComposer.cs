using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuMVC.Spark.SparkModel
{
    public interface ITemplateComposer
    {
        IEnumerable<ITemplate> Compose(TypePool typePool);
    }

    public class TemplateComposer : ITemplateComposer
    {
        private readonly IList<ITemplateBinder> _binders = new List<ITemplateBinder>();
        private readonly IList<ITemplatePolicy> _policies = new List<ITemplatePolicy>();
        private readonly ITemplates _templates;
        
        private readonly IChunkLoader _chunkLoader;

        public TemplateComposer(ITemplates templates) : this(templates, new ChunkLoader()) { }
        public TemplateComposer(ITemplates templates, IChunkLoader chunkLoader)
        {
            _templates = templates;
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

        public IEnumerable<ITemplate> Compose(TypePool typePool)
        {
            _templates.Each(t =>
            {
                var bindRequest = createBindRequest(t, typePool);

                var binders = _binders.Where(x => x.CanBind(bindRequest));
                var policies = _policies.Where(x => x.Matches(t));

                binders.Each(binder => binder.Bind(bindRequest));
                policies.Each(policy => policy.Apply(t));
            });

            return _templates;
        }

        private BindRequest createBindRequest(ITemplate template, TypePool typePool)
        {
            var chunks = _chunkLoader.Load(template);
            return new BindRequest
            {
                Target = template,
                Types = typePool,
                Templates = _templates,
                Master = chunks.Master(),
                ViewModelType = chunks.ViewModel(),
                Namespaces = chunks.Namespaces(),
                Logger = SparkLogger.Default()
            };
        }
    }    
}