using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using Spark;
using Spark.Compiler;
using Spark.FileSystem;
using Spark.Parser;
using Spark.Parser.Syntax;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkItemBuilder
    {
        IEnumerable<SparkItem> BuildItems(TypePool types);
    }

    public class SparkItemBuilder : ISparkItemBuilder
    {
        private readonly IList<ISparkItemBinder> _itemBinders = new List<ISparkItemBinder>();
        private readonly ISparkItemFinder _finder;
        private readonly IChunkLoader _chunkLoader;

        public SparkItemBuilder() : this(new SparkItemFinder(), new ChunkLoader()) {}
        public SparkItemBuilder(ISparkItemFinder finder, IChunkLoader chunkLoader)
        {
            _finder = finder;
            _chunkLoader = chunkLoader;
        }

        public SparkItemBuilder Apply<T>() where T : ISparkItemBinder, new()
        {
            return Apply<T>(c => { });
        }

        public SparkItemBuilder Apply<T>(Action<T> configure) where T : ISparkItemBinder, new()
        {
            var binder = new T();
            configure(binder);
            _itemBinders.Add(binder);
            return this;
        }

        public IEnumerable<SparkItem> BuildItems(TypePool types)
        {
            var items = new SparkItems();

            items.AddRange(_finder.FindItems());
            items.Each(item => _itemBinders.Each(binder =>
            {
                var chunks = _chunkLoader.Load(item);
                var context = createContext(chunks, types, items);
                binder.Bind(item, context);
            }));

            return items;
        }

        private BindContext createContext(IEnumerable<Chunk> chunks, TypePool types, SparkItems items)
        {
            var context = new BindContext
            {
                Master = chunks.OfType<UseMasterChunk>().FirstValue(x => x.Name),
                ViewModelType = chunks.OfType<ViewDataModelChunk>().FirstValue(x => x.TModel.ToString()),
                Namespaces = chunks.OfType<UseNamespaceChunk>().Select(x => x.Namespace.ToString()),
                TypePool = types,
                SparkItems = items
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