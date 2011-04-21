using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkItemBuilder
    {
        IEnumerable<SparkItem> BuildItems(TypePool types, BehaviorGraph graph);
    }

    public class SparkItemBuilder : ISparkItemBuilder
    {
        private readonly IList<ISparkItemBinder> _itemBinders = new List<ISparkItemBinder>();
        private readonly ISparkItemFinder _finder;
        private readonly IFileSystem _fileSystem;

        public SparkItemBuilder() : this(new SparkItemFinder(), new FileSystem()) {}
        public SparkItemBuilder(ISparkItemFinder finder, IFileSystem fileSystem)
        {
            _finder = finder;
            _fileSystem = fileSystem;
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

        public IEnumerable<SparkItem> BuildItems(TypePool types, BehaviorGraph graph)
        {
            var items = new SparkItems();

            items.AddRange(_finder.FindItems());
            items.Each(item => _itemBinders.Where(m => m.Applies(item)).Each(binder =>
            {
                var fileContent = _fileSystem.ReadStringFromFile(item.FilePath);
                var context = new BindContext
                {
                    TypePool = types,
                    SparkItems = items,
                    FileContent = fileContent
                };

                binder.Bind(item, context);
            }));

            return items;
        }
    }
}