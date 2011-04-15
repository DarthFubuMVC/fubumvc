using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.Tokenization.Model;

namespace FubuMVC.Spark.Tokenization
{
    public interface IViewTokenizer
    {
        IEnumerable<SparkViewToken> Tokenize(TypePool types, BehaviorGraph graph);
    }

    public class ViewTokenizer : IViewTokenizer
    {
        private readonly IList<ISparkFileEnricher> _enrichers = new List<ISparkFileEnricher>();
        private readonly ISparkFileSource _source;
        private readonly IFileSystem _fileSystem;

        public ViewTokenizer() : this(new SparkFileSource(), new FileSystem())
        {
            // Remove when we can get the tokenizer from the spark registry.
            _enrichers.AddRange(DefaultDependencies.Enrichers());
        }

        public ViewTokenizer(ISparkFileSource source, IFileSystem fileSystem)
        {
            _source = source;
            _fileSystem = fileSystem;
        }

        public IViewTokenizer AddEnricher<T>() where T :  ISparkFileEnricher, new()
        {
            var enricher = new T();
            _enrichers.Add(enricher);
            return this;
        }

        public IEnumerable<SparkViewToken> Tokenize(TypePool types, BehaviorGraph graph)
        {
            return GetFilesWithModel(types).Select(file => new SparkViewToken(file));
        }

        public IEnumerable<SparkFile> GetFilesWithModel(TypePool types)
        {
            var files = new SparkFiles();

            files.AddRange(_source.GetFiles());
            files.Each(file => _enrichers.Each(enricher =>
            {
                var fileContent = _fileSystem.ReadStringFromFile(file.Path);
                var context = new EnrichmentContext
                {
                    TypePool = types,
                    SparkFiles = files,
                    FileContent = fileContent
                };

                enricher.Enrich(file, context);
            }));

            return files.Where(f => f.HasViewModel());
        }
    }
}