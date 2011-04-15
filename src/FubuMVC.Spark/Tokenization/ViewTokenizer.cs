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
        private readonly ISparkFilesProvider _sparkFilesProvider;

        public ViewTokenizer(ISparkFilesProvider sparkFilesProvider)
        {
            _sparkFilesProvider = sparkFilesProvider;
        }

        public IEnumerable<SparkViewToken> Tokenize(TypePool types, BehaviorGraph graph)
        {
            var validFiles = _sparkFilesProvider.GetFilesWithModel(types);            
            var validActions = graph.Actions().Where(x => x.HasOutput).ToLookup(x => x.OutputType());

            foreach (var sparkFile in validFiles)
            {
                foreach (var action in validActions[sparkFile.ViewModelType])
                {
                    yield return new SparkViewToken(sparkFile, action);
                }
            }
        }
    }

    // The name is a bit "cheesy"
    public interface ISparkFilesProvider
    {
        IEnumerable<SparkFile> GetFilesWithModel(TypePool types);
    }

    public class SparkFileProvider : ISparkFilesProvider
    {
        private readonly ISparkFileSource _source;
        private readonly IFileSystem _fileSystem;
        private readonly IEnumerable<ISparkFileEnricher> _enrichers;

        public SparkFileProvider(ISparkFileSource source, IFileSystem fileSystem, IEnumerable<ISparkFileEnricher> enrichers)
        {
            _source = source;
            _fileSystem = fileSystem;
            _enrichers = enrichers;
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