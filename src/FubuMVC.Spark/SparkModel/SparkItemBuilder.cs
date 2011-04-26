using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using Spark;
using Spark.Compiler;
using Spark.FileSystem;
using Spark.Parser;
using Spark.Parser.Syntax;

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
        private readonly SparkItems _sparkItems;
        private readonly IChunkLoader _chunkLoader;

        public SparkItemBuilder(SparkItems sparkItems) : this(sparkItems, new ChunkLoader()) {}
        public SparkItemBuilder(SparkItems sparkItems, IChunkLoader chunkLoader)
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

        public SparkItemBuilder Apply<T>() where T : ISparkItemPolicy, new()
        {
            return Apply<T>(c => { });
        }

        public SparkItemBuilder Apply<T>(Action<T> configure) where T : ISparkItemPolicy, new()
        {
            var policy = new T();
            configure(policy);
            _policies.Add(policy);
            return this;
        }

        public void BuildItems()
        {
            _sparkItems.Each(item => _itemBinders.Each(binder =>
            {
                var chunks = _chunkLoader.Load(item);
                var context = createContext(chunks);

                binder.Bind(item, context);
                
                _policies.Where(p => p.Matches(item))
                    .Each(p => p.Apply(item));
            }));
        }

        // Get away from this
        private BindContext createContext(IEnumerable<Chunk> chunks)
        {
            var context = new BindContext
            {
                AvailableItems = _sparkItems,
                Master = chunks.OfType<UseMasterChunk>().FirstValue(x => x.Name),
                ViewModelType = chunks.OfType<ViewDataModelChunk>().FirstValue(x => x.TModel.ToString()),
                Namespaces = chunks.OfType<UseNamespaceChunk>().Select(x => x.Namespace.ToString())                
            };

            return context;
        }
    }

    public interface IChunkLoader
    {
        IEnumerable<Chunk> Load(SparkItem item);
    }

    // TODO: Improve testability
    public class ChunkLoader : IChunkLoader
    {
        private readonly Cache<string, ViewLoader> _loaders;
        private readonly ISparkSyntaxProvider _syntaxProvider;

        public ChunkLoader()
        {
            _loaders = new Cache<string, ViewLoader>(viewLoaderByRoot);
            _syntaxProvider = new DefaultSyntaxProvider(ParserSettings.DefaultBehavior);
        }

        public IEnumerable<Chunk> Load(SparkItem item)
        {
            return _loaders[item.RootPath].Load(item.RelativePath()).ToList();
        }

        private ViewLoader viewLoaderByRoot(string root)
        {
            return new ViewLoader
            {
                SyntaxProvider = _syntaxProvider,
                ViewFolder = new FileSystemViewFolder(root)
            };            
        }
    }
}