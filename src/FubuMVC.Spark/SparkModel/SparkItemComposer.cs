using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using Spark.Compiler;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkItemComposer
    {
        IEnumerable<ITemplate> ComposeViews(TypePool typePool);
    }

    public class SparkItemComposer : ISparkItemComposer
    {
        private readonly IList<ISparkTemplateBinder> _itemBinders = new List<ISparkTemplateBinder>();
        private readonly IList<ISparkTemplatePolicy> _policies = new List<ISparkTemplatePolicy>();
        private readonly IEnumerable<ITemplate> _templates;
        private readonly IChunkLoader _chunkLoader;

        public SparkItemComposer(IEnumerable<ITemplate> templates) : this(templates, new ChunkLoader()) { }
        public SparkItemComposer(IEnumerable<ITemplate> templates, IChunkLoader chunkLoader)
        {
            // TODO : I think we need to get a list of ITemplate in here. Enriched (ViewPath set) and work from that.
            // In other words, separate the template from sparkitem, and let the composer create the items.
            _templates = templates;
            _chunkLoader = chunkLoader;
        }

        public SparkItemComposer AddBinder<T>() where T : ISparkTemplateBinder, new()
        {
            var binder = new T();
            return AddBinder(binder);
        }

        public SparkItemComposer AddBinder(ISparkTemplateBinder binder)
        {
            _itemBinders.Add(binder);
            return this;
        }
        public SparkItemComposer Apply(ISparkTemplatePolicy policy)
        {
            _policies.Add(policy);
            return this;
        }

        public SparkItemComposer Apply<T>() where T : ISparkTemplatePolicy, new()
        {
            return Apply(new T());
        }

        public SparkItemComposer Apply<T>(Action<T> configure) where T : ISparkTemplatePolicy, new()
        {
            var policy = new T();
            configure(policy);
            Apply(policy);
            return this;
        }

        public IEnumerable<ITemplate> ComposeViews(TypePool typePool)
        {
            _templates.Each(item =>
            {
                var chunks = _chunkLoader.Load(item);
                var context = createContext(chunks, typePool);
                var binders = _itemBinders.Where(x => x.CanBind(item, context));
                var policies = _policies.Where(x => x.Matches(item));

                binders.Each(binder => binder.Bind(item, context));
                policies.Each(policy => policy.Apply(item));
            });

            return _templates;
        }

        // TODO: Meh, find better way
        private BindContext createContext(IEnumerable<Chunk> chunks, TypePool typePool)
        {
            return new BindContext
            {
                TypePool = typePool,
                AvailableTemplates = _templates,
                Master = chunks.Master(),
                ViewModelType = chunks.ViewModel(),
                Namespaces = chunks.Namespaces(),
                Tracer = SparkTracer.Default()
            };
        }
    }    
}