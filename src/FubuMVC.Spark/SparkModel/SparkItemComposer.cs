using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using Spark.Compiler;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkItemComposer
    {
        IEnumerable<SparkItem> ComposeViews(TypePool typePool);
    }

    public class SparkItemComposer : ISparkItemComposer
    {
        private readonly IList<ISparkItemBinder> _itemBinders = new List<ISparkItemBinder>();
        private readonly IList<ISparkItemPolicy> _policies = new List<ISparkItemPolicy>();
        private readonly IEnumerable<SparkItem> _sparkItems;
        private readonly IChunkLoader _chunkLoader;

        public SparkItemComposer(IEnumerable<SparkItem> sparkItems) : this(sparkItems, new ChunkLoader()) { }
        public SparkItemComposer(IEnumerable<SparkItem> sparkItems, IChunkLoader chunkLoader)
        {
            _sparkItems = sparkItems;
            _chunkLoader = chunkLoader;
        }

        public SparkItemComposer AddBinder<T>() where T : ISparkItemBinder, new()
        {
            var binder = new T();
            return AddBinder(binder);
        }

        public SparkItemComposer AddBinder(ISparkItemBinder binder)
        {
            _itemBinders.Add(binder);
            return this;
        }
        public SparkItemComposer Apply(ISparkItemPolicy policy)
        {
            _policies.Add(policy);
            return this;
        }

        public SparkItemComposer Apply<T>() where T : ISparkItemPolicy, new()
        {
            return Apply(new T());
        }

        public SparkItemComposer Apply<T>(Action<T> configure) where T : ISparkItemPolicy, new()
        {
            var policy = new T();
            configure(policy);
            Apply(policy);
            return this;
        }

        public IEnumerable<SparkItem> ComposeViews(TypePool typePool)
        {
            _sparkItems.Each(item =>
            {
                var chunks = _chunkLoader.Load(item);
                var context = createContext(chunks, typePool);
                var binders = _itemBinders.Where(x => x.CanBind(item, context));
                var policies = _policies.Where(x => x.Matches(item));

                binders.Each(binder => binder.Bind(item, context));
                policies.Each(policy => policy.Apply(item));
            });

            return _sparkItems;
        }

        // TODO: Meh, find better way
        private BindContext createContext(IEnumerable<Chunk> chunks, TypePool typePool)
        {
            return new BindContext
            {
                TypePool = typePool,
                AvailableItems = _sparkItems,
                Master = chunks.Master(),
                ViewModelType = chunks.ViewModel(),
                Namespaces = chunks.Namespaces(),
                Tracer = SparkTracer.Default()
            };
        }
    }    
}