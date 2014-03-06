using System.Linq;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkTemplate : ITemplateFile {}

    public class SparkTemplate : Template, ISparkTemplate
    {
        private static readonly ChunkLoader Loader = new ChunkLoader();

        public SparkTemplate(IFubuFile file) : base(file)
        {
        }

        public SparkTemplate(string filePath, string rootPath, string origin) : base(filePath, rootPath, origin)
        {
        }

        protected override Parsing createParsing()
        {
            var chunk = Loader.Load(this).ToList();

            return new Parsing
            {
                Master = chunk.Master(),
                ViewModelType = chunk.ViewModel(),
                Namespaces = chunk.Namespaces()
            };
        }
    }

}