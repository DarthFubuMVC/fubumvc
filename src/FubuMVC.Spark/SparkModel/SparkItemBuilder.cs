using System;
using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;
using Spark.Compiler;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkItemBuilder
    {
        void BuildItems();
    }

    public class SparkItemBuilder : ISparkItemBuilder
    {
        private readonly IList<ISparkItemBinder> _itemBinders = new List<ISparkItemBinder>();
        private readonly IList<ISparkItemPolicy> _policies = new List<ISparkItemPolicy>();
        private readonly IEnumerable<SparkItem> _sparkItems;
        private readonly IChunkLoader _chunkLoader;

        public SparkItemBuilder(IEnumerable<SparkItem> sparkItems) : this(sparkItems, new ChunkLoader()) { }
        public SparkItemBuilder(IEnumerable<SparkItem> sparkItems, IChunkLoader chunkLoader)
        {
            _sparkItems = sparkItems;
            _chunkLoader = chunkLoader;
        }

        public SparkItemBuilder AddBinder<T>() where T : ISparkItemBinder, new()
        {
            var binder = new T();
            return AddBinder(binder);
        }

        public SparkItemBuilder AddBinder(ISparkItemBinder binder)
        {
            _itemBinders.Add(binder);
            return this;
        }
        public SparkItemBuilder Apply(ISparkItemPolicy policy)
        {
            _policies.Add(policy);
            return this;
        }

        public SparkItemBuilder Apply<T>() where T : ISparkItemPolicy, new()
        {
            return Apply(new T());
        }

        public SparkItemBuilder Apply<T>(Action<T> configure) where T : ISparkItemPolicy, new()
        {
            var policy = new T();
            configure(policy);
            Apply(policy);
            return this;
        }

        public void BuildItems()
        {
            _sparkItems.Each(item => _itemBinders.All(b => b.CanBind(item)).Each(binder =>
            {
                var chunks = _chunkLoader.Load(item);
                var context = createContext(chunks);

                binder.Bind(item, context);

                _policies.Where(p => p.Matches(item)).Each(p => p.Apply(item));
            }));
        }

        private BindContext createContext(IEnumerable<Chunk> chunks)
        {
            return new BindContext
            {
                AvailableItems = _sparkItems,
                Master = chunks.Master(),
                ViewModelType = chunks.ViewModel(),
                Namespaces = chunks.Namespaces(),
                Tracer = SparkPackageTracer.Default()
            };
        }
    }

    public interface ISparkPackageTracer
    {
        void Trace(SparkItem item, string format, params object [] args);
        void Trace(SparkItem item, string text);
    }

    public class SparkPackageTracer : ISparkPackageTracer
    {
        private readonly Action<SparkItem, string, object[]> _format;
        private readonly Action<SparkItem ,string> _trace;

        public SparkPackageTracer(Action<SparkItem, string, object[]> format, Action<SparkItem, string> trace)
        {
            _format = format;
            _trace = trace;
        }

        public void Trace(SparkItem item, string format, params object [] args)
        {
            _format(item,format, args);
        }

        public void Trace(SparkItem item, string text)
        {
            _trace(item, text);
        }

        public static SparkPackageTracer Default()
        {
            return new SparkPackageTracer(formatTrace, trace);
        }

        private static PackageLog getPackageLogger(SparkItem item)
        {
            return PackageRegistry.Diagnostics.LogFor(item);
        }

        private static void formatTrace(SparkItem item, string format, object[] args)
        {
            getPackageLogger(item).Trace(format, args);
        }

        private static void trace(SparkItem item, string text)
        {
            getPackageLogger(item).Trace(text);
        }
    }

}