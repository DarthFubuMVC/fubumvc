using System.Collections.Generic;
using System.Linq;
using Bottles;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.Tokenization.Model;
using FubuMVC.Spark.Tokenization.Parsing;
using FubuMVC.Spark.Tokenization.Scanning;

namespace FubuMVC.Spark.Tokenization
{
    public interface IViewTokenizer
    {
        IEnumerable<SparkViewToken> Tokenize(TypePool types, BehaviorGraph graph);
    }
    
    public class ViewTokenizer : IViewTokenizer
    {
        private readonly ISparkFileSource _source;
        private readonly IFileSystem _fileSystem;
        private readonly IEnumerable<ISparkFileEnricher> _enrichers;

        public ViewTokenizer()
            : this(DefaultDependencies.SparkFileSource(), new FileSystem(), DefaultDependencies.Enrichers())
        {
        }

        public ViewTokenizer(ISparkFileSource source, IFileSystem fileSystem, IEnumerable<ISparkFileEnricher> enrichers)
        {
            _enrichers = enrichers;
            _fileSystem = fileSystem;
            _source = source;
        }

        public IEnumerable<SparkViewToken> Tokenize(TypePool types, BehaviorGraph graph)
        {
            var sparkFiles = getFilesWithModel(types);

            return sparkFiles.Select(sparkFile => new SparkViewToken(sparkFile));
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

    // NOTE:TEMP
    public static class DefaultDependencies
    {
        public static ISparkFileSource SparkFileSource()
        {
            return new SparkFileSource(new FileScanner(new FileSystem()), PackageRegistry.Packages);
        }
        public static IEnumerable<ISparkFileEnricher> Enrichers()
        {
            yield return new MasterPageEnricher(new SparkParser(new ElementNodeExtractor()));
            yield return new ViewModelEnricher(new SparkParser(new ElementNodeExtractor()));
            yield return new NamespaceEnricher();
        }
    }

}