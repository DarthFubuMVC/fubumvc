using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;

namespace FubuMVC.Spark.Tokenization
{
    public interface IViewTokenizer
    {
        IEnumerable<SparkViewToken> Tokenize(TypePool types, BehaviorGraph graph);
    }

    public class ViewTokenizer : IViewTokenizer
    {
        private readonly IList<ISparkItemModifier> _itemModifiers = new List<ISparkItemModifier>();
        private readonly ISparkItemFinder _finder;
        private readonly IFileSystem _fileSystem;

        public ViewTokenizer() : this(new SparkItemFinder(), new FileSystem()) {}
        public ViewTokenizer(ISparkItemFinder finder, IFileSystem fileSystem)
        {
            _finder = finder;
            _fileSystem = fileSystem;
        }

        public ViewTokenizer AddModifier<T>() where T : ISparkItemModifier, new()
        {
            return AddModifier<T>(c => { });
        }

        public ViewTokenizer AddModifier<T>(Action<T> configure) where T : ISparkItemModifier, new()
        {
            var modifier = new T();
            configure(modifier);
            _itemModifiers.Add(modifier);
            return this;
        }

        public IEnumerable<SparkViewToken> Tokenize(TypePool types, BehaviorGraph graph)
        {
            return getItemsWithModel(types).Select(item => new SparkViewToken(item));
        }

        private IEnumerable<SparkItem> getItemsWithModel(TypePool types)
        {
            var items = new SparkItems();

            items.AddRange(_finder.FindItems());
            items.Each(item => _itemModifiers.Each(modifier =>
            {
                var fileContent = _fileSystem.ReadStringFromFile(item.Path);
                var context = new EnrichmentContext
                {
                    TypePool = types,
                    SparkItems = items,
                    FileContent = fileContent
                };

                modifier.Modify(item, context);
            }));

            return items.Where(f => f.HasViewModel());
        }
    }
}