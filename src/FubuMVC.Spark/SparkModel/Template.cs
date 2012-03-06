using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public interface ITemplate : ITemplateFile
    {
        string FilePath { get; }
        string RootPath { get; }
        string Origin { get; }
		
        string ViewPath { get; set; }
        ISparkDescriptor Descriptor { get; set; }
    }

    public class Template : ITemplate
    {
        public Template(string filePath, string rootPath, string origin)
        {
            FilePath = filePath;
            RootPath = rootPath;
            Origin = origin;

            Descriptor = new NulloDescriptor();
        }

        public string FilePath { get; private set; }
        public string RootPath { get; private set; }
        public string Origin { get; private set; }
		
        public string ViewPath { get; set; }
        public ISparkDescriptor Descriptor { get; set; }

	    public override string ToString()
        {
            return FilePath;
        }
    }

    public class Parsings : IParsingRegistrations<ITemplate>
    {
        private readonly Cache<string, Parsing> _parsings = new Cache<string, Parsing>();
        private readonly IChunkLoader _chunkLoader;

        public Parsings() : this(new ChunkLoader()){}
        public Parsings(IChunkLoader chunkLoader)
        {
            _chunkLoader = chunkLoader;
        }

        public void Process(ITemplate template)
        {
            var chunk = _chunkLoader.Load(template).ToList();

            _parsings[template.FilePath] = new Parsing
            {
               Master = chunk.Master(),
               ViewModelType = chunk.ViewModel(),
               Namespaces = chunk.Namespaces()
            };
        }

        public Parsing ParsingFor(ITemplate template)
        {
            return _parsings[template.FilePath];
        }
    }
}