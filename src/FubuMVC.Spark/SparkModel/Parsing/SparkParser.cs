using System.Collections.Generic;

namespace FubuMVC.Spark.SparkModel.Parsing
{
    public interface ISparkParser
    {
        string Parse(string templateContent, string node, string attribute);
    }

    public class SparkParser : ISparkParser
    {
        private readonly IElementNodeExtractor _nodeExtractor;

        public SparkParser() : this(new ElementNodeExtractor()) {}
        public SparkParser(IElementNodeExtractor nodeExtractor)
        {
            _nodeExtractor = nodeExtractor;
        }

        public string Parse(string templateContent, string node, string attribute)
        {
            return _nodeExtractor
                .ExtractByName(templateContent, node)
                .FirstValue(n => n.AttributeByName(attribute));
        }
    }

    public static class SparkParserExtensions
    {
        public static string ParseViewModelTypeName(this ISparkParser parser, string content)
        {
            return parser.Parse(content, "viewdata", "model");
        }

        public static string ParseMasterName(this ISparkParser parser, string content)
        {
            return parser.Parse(content, "use", "master");
        }
    }
}