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
            var sparkFiles = _sparkFilesProvider.GetFiles(types);
            var validActions = graph.Actions().Where(x => x.HasOutput).ToLookup(x => x.OutputType());
            foreach (var sparkFile in sparkFiles)
            {
                foreach (var action in validActions[sparkFile.ViewModelType])
                {
                    yield return new SparkViewToken(sparkFile, action);
                }
            }
        }
    }

    public interface ISparkFilesProvider
    {
        IEnumerable<SparkFile> GetFiles(TypePool typePool);
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

        public IEnumerable<SparkFile> GetFiles(TypePool typePool)
        {
            var sparkFiles = new SparkFiles();

            sparkFiles.AddRange(_source.GetFiles());

            foreach (var sparkFile in sparkFiles)
            {
                applyEnrichers(sparkFiles, typePool, sparkFile);
            }

            return sparkFiles.Where(x => x.HasViewModel());
        }

        private void applyEnrichers(SparkFiles sparkFiles, TypePool typePool, SparkFile sparkFile)
        {
            foreach (var enricher in _enrichers)
            {
                var context = getContext(sparkFiles, typePool, sparkFile);
                enricher.Enrich(sparkFile, context);
            }
        }

        private EnrichmentContext getContext(SparkFiles sparkFiles, TypePool typePool, SparkFile sparkFile)
        {
            return new EnrichmentContext
            {
                SparkFiles = sparkFiles,
                TypePool = typePool,
                FileContent = _fileSystem.ReadStringFromFile(sparkFile.Path)
            };
        }
    }
}