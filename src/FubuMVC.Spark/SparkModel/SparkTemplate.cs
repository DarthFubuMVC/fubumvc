using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkTemplate : ITemplateFile {}
    public class SparkTemplate : Template, ISparkTemplate
    {
        public SparkTemplate(IFubuFile file) : base(file)
        {
        }

        public SparkTemplate(string filePath, string rootPath, string origin) : base(filePath, rootPath, origin)
        {
        }
    }

    public class SparkParsings : IParsingRegistrations<ISparkTemplate>
    {
        private readonly Cache<string, Parsing> _parsings = new Cache<string, Parsing>();
        private readonly IChunkLoader _chunkLoader;

        public SparkParsings() : this(new ChunkLoader()){}
        public SparkParsings(IChunkLoader chunkLoader)
        {
            _chunkLoader = chunkLoader;
        }

        public void Process(ISparkTemplate template)
        {
            var chunk = _chunkLoader.Load(template).ToList();

            _parsings[template.FilePath] = new Parsing
            {
               Master = chunk.Master(),
               ViewModelType = chunk.ViewModel(),
               Namespaces = chunk.Namespaces()
            };
        }

        public Parsing ParsingFor(ISparkTemplate template)
        {
            return _parsings[template.FilePath];
        }
    }
}