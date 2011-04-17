using System;
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

        public ViewTokenizer() : this(new SparkFileSource(), new FileSystem()) {}
        public ViewTokenizer(ISparkFileSource source, IFileSystem fileSystem)
        {
            _source = source;
            _fileSystem = fileSystem;
        }

        public ViewTokenizer AddEnricher<T>() where T : ISparkFileEnricher, new()
        {
            return AddEnricher<T>(c => { });
        }

        public ViewTokenizer AddEnricher<T>(Action<T> configure) where T : ISparkFileEnricher, new()
        {
            var enricher = new T();
            configure(enricher);
            _enrichers.Add(enricher);
            return this;
        }

        public IEnumerable<SparkViewToken> Tokenize(TypePool types, BehaviorGraph graph)
        {
            return getFilesWithModel(types).Select(file => new SparkViewToken(file));
        }

        private IEnumerable<SparkFile> getFilesWithModel(TypePool types)
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